﻿using System;
using System.Net;
using System.Threading;
using Everest.Collections;
using Everest.Http;
using Everest.Middlewares;
using Everest.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Everest.Rest
{
    public class RestServer : IDisposable, IHasLogger
	{
        ILogger IHasLogger.Logger => Logger;

        public ILogger<RestServer> Logger { get; }

        public event EventHandler Started;

        public event EventHandler Stopped;
		
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

		private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

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

			if (IsStopping || IsStarting || IsListening)
				return;

			IsStarting = true;

			var exceptionWasThrown = false;

			try
			{
                Logger.LogTraceIfEnabled(() => $"Starting server at {Prefixes.ToReadableArray()}");

				listener.Start();
				listenerThread.Start();

                Logger.LogTraceIfEnabled(() => "Server is started");
			}
			catch (Exception ex)
			{
				exceptionWasThrown = true;

                Logger.LogCriticalIfEnabled(() => (ex, "Failed while starting server"));
				throw;
			}
			finally
			{
				IsStarting = false;

				if (exceptionWasThrown)
					Dispose();
				else
					Started?.Invoke(this, EventArgs.Empty);
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
                Logger.LogTraceIfEnabled(() => "Stopping server");

				listener.Stop();
				listener.Close();
				cancellationTokenSource.Cancel();
            }
			catch (ObjectDisposedException)
			{
				//Ignore
			}
			catch (Exception ex)
			{
                Logger.LogErrorIfEnabled(() => (ex, "Failed while stopping server"));
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
                Logger.LogErrorIfEnabled(() => (ex, "Failed while stopping server"));
			}

            try
            {
                IsStopping = false;
                Logger.LogTraceIfEnabled(() => "Server is stopped");
            }
            finally
            {
                Stopped?.Invoke(this, EventArgs.Empty);
            }
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
                    var httpContext = new HttpContext(context, features, services, loggerFactory, cancellationTokenSource.Token);

#if NET5_0_OR_GREATER
					ThreadPool.QueueUserWorkItem(ProcessRequestAsync, httpContext, false);
#else
                    ThreadPool.QueueUserWorkItem(ProcessRequestAsync, httpContext);
#endif
                }
				catch (HttpListenerException ex) when (ex.ErrorCode == 995 && (IsStopping || !IsListening))
				{
                    Logger.LogWarningIfEnabled(() => (ex, $"{ex.ErrorCode}"));
				}
				catch (Exception ex)
				{
                    Logger.LogErrorIfEnabled(() => (ex, "Failed to process incoming request"));
				}
			}
		}

#if !NET5_0_OR_GREATER
		private void ProcessRequestAsync(object state)
        {
			ProcessRequestAsync(state as IHttpContext);
        }
#endif
		private async void ProcessRequestAsync(IHttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			var exceptionWasThrown = false;

            try
            {
                Logger.LogTraceIfEnabled(() => $"{context.TraceIdentifier} - Try to process incoming request");
                await aggregateMiddleware.InvokeAsync(context);
            }
            catch (HttpListenerException ex)
            {
                exceptionWasThrown = true;
                Logger.LogErrorIfEnabled(() => (ex, $"{context.TraceIdentifier} - Failed to process incoming request"));
            }
            catch (Exception ex)
            {
                exceptionWasThrown = true;

                Logger.LogErrorIfEnabled(() => (ex, $"{context.TraceIdentifier} - Failed to process incoming request"));
                context.Response.StatusCode = HttpStatusCode.InternalServerError;
            }
            finally
            {
                if (!context.Response.ResponseSent)
                {
                    try
                    {
                        context.Response.OutputStream.Close();
                    }
                    catch (Exception ex)
                    {
                        exceptionWasThrown = true;
                        Logger.LogErrorIfEnabled(() => (ex, $"{context.TraceIdentifier} - Failed to close output stream"));
                    }
                    finally
                    {
                        context.Response.CloseResponse();
                    }
                }

                if (!exceptionWasThrown)
                {
                    Logger.LogTraceIfEnabled(() => $"{context.TraceIdentifier} - Successfully processed incoming request");
                }
            }
        }

		public void Dispose()
		{
			if (IsDisposed)
				return;

			try
			{
				Stop();
				cancellationTokenSource.Dispose();
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
