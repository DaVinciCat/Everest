using Everest.Http;
using System;
using System.Net;
using System.Threading;
using Everest.Middleware;
using Everest.Routing;
using Everest.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Everest
{
	public class RestServer : IDisposable
	{
		public ILogger<RestServer> Logger { get; }

		public bool IsDisposed { get; private set; }

		public bool IsStopping { get; private set; }

		public bool IsStarting { get; private set; }

		public bool IsListening => listener.IsListening;

		internal IServiceProvider ServiceProvider { get; }

		internal AggregateMiddleware Middleware { get; } = new();

		public PrefixCollection Prefixes { get; }
		
		private readonly HttpListener listener;

		private readonly Thread listenerThread;
		
		public RestServer(IServiceProvider serviceProvider, ILogger<RestServer> logger)
		{
			if (!HttpListener.IsSupported)
			{
				throw new NotSupportedException($"This OS does not support {nameof(HttpListener)} class.");
			}

			ServiceProvider = serviceProvider;
			Logger = logger;

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

			try
			{
				Logger.LogTrace($"Starting server at {string.Join("; ", Prefixes)}");
				listener.Start();
				listenerThread.Start();
				Logger.LogTrace("Server is started");
			}
			catch (Exception ex)
			{
				Logger.LogCritical(ex, "Failed while starting server");
				throw;
			}
			finally
			{
				IsStarting = false;
			}
		}

		private void Stop()
		{
			if (IsDisposed)
				throw new ObjectDisposedException(GetType().FullName);

			if (IsStopping)
				return;

			try
			{
				IsStopping = true;
				Logger.LogTrace("Stopping server");
				listener.Stop();
				listener.Close();
				listenerThread.Join();
				Logger.LogTrace("Server is stopped");
			}
			catch (Exception ex)
			{
				Logger.LogCritical(ex, "Failed while stopping server");
				throw;
			}
			finally
			{
				IsStopping = false;
			}
		}

		private async void ListenAsync()
		{
			while (listener.IsListening)
			{
				try
				{
					var context = await listener.GetContextAsync();
					var request = new HttpRequest(context.Request);
					var response = new HttpResponse(context.Response);
					var services = ServiceProvider.CreateScope().ServiceProvider;
					var httpContext = new HttpContext(request, response, services);
						
					ThreadPool.QueueUserWorkItem(ProcessRequestAsync, httpContext, false);
				}
				catch (HttpListenerException ex) when (ex.ErrorCode == 995 && (IsStopping || !IsListening))
				{
					Logger.LogWarning(ex, $"{ex.ErrorCode}");
				}
				catch (Exception ex)
				{
					Logger.LogCritical(ex, "Failed to process incoming request");
				}
			}
		}

		private void ProcessRequestAsync(HttpContext context)
		{
			try
			{
				Middleware.Invoke(context);

				if (!context.Response.IsSent && !context.Response.IsClosed)
					context.Response.Send();
			}
			catch (Exception ex)
			{
				Logger.LogCritical(ex, "Failed to process incoming request");
			}
			finally
			{
				context.Response.Close();
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

		public static RestServer UseMiddleware(this RestServer server, IMiddleware middleware)
		{
			server.Middleware.AddMiddleware(middleware);
			return server;
		}

		public static RestServer UseExceptionHandlingMiddleware(this RestServer server, ILogger<ExceptionHandlingMiddleware> logger)
		{
			server.Middleware.AddMiddleware(new ExceptionHandlingMiddleware(logger));
			return server;
		}

		public static RestServer UseRoutingMiddleware(this RestServer server, IRouter router)
		{
			server.Middleware.AddMiddleware(new RoutingMiddleware(router));
			return server;
		}

		public static RestServer UseCompressionMiddleware(this RestServer server, ICompressionProvider provider)
		{
			server.Middleware.AddMiddleware(new CompressionMiddleware(provider));
			return server;
		}

		public static RestServer UseCorsMiddleware(this RestServer server)
		{
			server.Middleware.AddMiddleware(new CorsMiddleware());
			return server;
		}
	}
}
