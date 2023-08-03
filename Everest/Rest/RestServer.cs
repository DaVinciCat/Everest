using Everest.Http;
using System;
using System.Net;
using System.Threading;
using Everest.Collections;
using Everest.Middleware;
using Everest.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Everest.Rest
{
	public class RestServer : IDisposable
	{
		public ILogger<RestServer> Logger { get; }

		public bool IsDisposed { get; private set; }

		public bool IsStopping { get; private set; }

		public bool IsStarting { get; private set; }

		public bool IsListening => listener.IsListening;

		public PrefixCollection Prefixes { get; }

		private readonly HttpListener listener;

		private readonly Thread listenerThread;

		private readonly AggregateMiddleware aggregateMiddleware;

		private readonly IServiceProvider serviceProvider;

		private readonly ILoggerFactory loggerFactory;

		public RestServer(IMiddleware[] middleware, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
		{
			if (!HttpListener.IsSupported)
			{
				throw new NotSupportedException($"This OS does not support {nameof(HttpListener)}.");
			}

			if (middleware == null)
				throw new ArgumentNullException(nameof(middleware));
			
			this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
			this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

			aggregateMiddleware = new AggregateMiddleware(middleware);

			Logger = loggerFactory.CreateLogger<RestServer>();

			listener = new HttpListener();
			listenerThread = new Thread(ListenAsync);

			Prefixes = new PrefixCollection(listener.Prefixes);
		}

		public void Start()
		{
			if (IsDisposed)
				throw new ObjectDisposedException(GetType().FullName);

			if (IsStopping || IsStarting)
				return;

			IsStarting = true;

			var exceptionWasThrown = false;

			try
			{
				Logger.LogTrace($"Starting server at {Prefixes.ToReadableArray()}");
				listener.Start();
				listenerThread.Start();
				Logger.LogTrace("Server is started");
			}
			catch (Exception ex)
			{
				exceptionWasThrown = true;

				Logger.LogCritical(ex, "Failed while starting server");
				throw;
			}
			finally
			{
				IsStarting = false;

				if (exceptionWasThrown)
					Dispose();
			}
		}

		private void Stop()
		{
			if (IsDisposed)
				throw new ObjectDisposedException(GetType().FullName);

			if (IsStopping)
				return;

			IsStopping = true;
			
			try
			{
				Logger.LogTrace("Stopping server");
				listener.Stop();
				listener.Close();
			}
			catch (ObjectDisposedException)
			{
				//Ignore
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Failed while stopping server");
			}

			try
			{
				listenerThread.Join();
			}
			catch (ThreadStateException)
			{
				//Ignore
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Failed while stopping server");
			}

			IsStopping = false;
			Logger.LogTrace("Server is stopped");
		}

		private async void ListenAsync()
		{
			while (listener.IsListening)
			{
				try
				{
					var context = await listener.GetContextAsync();
					var features = new FeatureCollection();
					var services = serviceProvider.CreateScope().ServiceProvider;
					var httpContext = new HttpContext(context, features, services, loggerFactory);

					ThreadPool.QueueUserWorkItem(ProcessRequestAsync, httpContext, false);
				}
				catch (HttpListenerException ex) when (ex.ErrorCode == 995 && (IsStopping || !IsListening))
				{
					Logger.LogWarning(ex, $"{ex.ErrorCode}");
				}
				catch (Exception ex)
				{
					Logger.LogError(ex, "Failed to process incoming request");
				}
			}
		}

		private async void ProcessRequestAsync(HttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			try
			{
				try
				{
					await aggregateMiddleware.InvokeAsync(context);

					if (!context.Response.ResponseSent)
					{
						await context.Response.SendAsync();
					}
				}
				finally
				{
					if (!context.Response.ResponseClosed)
					{
						await context.Response.OutputStream.DisposeAsync();
					}
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, $"{context.TraceIdentifier} - Failed to process incoming request");
			}
		}

		public void Dispose()
		{
			if (IsDisposed)
				return;

			try
			{
				Stop();
			}
			finally
			{
				IsDisposed = true;
			}
		}
	}

	public static class RestServerExtensions
	{
		public static RestServer AddPrefix(this RestServer server, string prefix)
		{
			server.Prefixes.Add(prefix);
			return server;
		}

		public static RestServer AddPrefixes(this RestServer server, string[] prefixes)
		{
			foreach (var prefix in prefixes)
			{
				server.Prefixes.Add(prefix);
			}

			return server;
		}
	}
}
