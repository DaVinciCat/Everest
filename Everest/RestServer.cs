using Everest.Http;
using Everest.Routing;
using System;
using System.Net;
using System.Threading;
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

		public IRouter Router { get; }

		public IRouteScanner RouteScanner { get; }

		public IServiceProvider ServiceProvider { get; private set; }

		public IServiceCollection Services { get; }

		public ICompressionProvider CompressionProvider { get; set; } = new CompressionProvider();

		public PrefixCollection Prefixes { get; }

		private readonly HttpListener listener;

		private readonly Thread listenerThread;

		public RestServer(IRouter router, IRouteScanner scanner, IServiceCollection services, ILogger<RestServer> logger)
		{
			if (!HttpListener.IsSupported)
			{
				throw new NotSupportedException($"This OS does not support {nameof(HttpListener)} class.");
			}

			Router = router;
			RouteScanner = scanner;
			Services = services;
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

					if (ServiceProvider == null)
						ServiceProvider = Services.BuildServiceProvider();

					var request = new HttpRequest(context.Request);
					var response = new HttpResponse(context.Response, CompressionProvider.GetCompression(request));
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
			Router.Route(context);
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
}
