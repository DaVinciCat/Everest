using Everest.Http;
using Everest.Routing;
using System;
using System.Net;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Everest
{
	public class RestServer: IDisposable
	{
		public bool IsDisposed { get; private set; }

		public bool IsStopping { get; private set; }

		public bool IsStarting { get; private set; }

		public bool IsListening => listener.IsListening;

		public Router Router { get; set; }

		public RouteScanner RouteScanner { get; set; }

		public IServiceProvider ServiceProvider { get; set; }

		public IServiceCollection Services { get; set; } = new ServiceCollection();

		private readonly HttpListener listener;

		private readonly Thread listenerThread;
		
		private readonly ILogger<RestServer> logger;
		
		public RestServer(Router router, RouteScanner routeScanner, ILogger<RestServer> logger)
		{ 
			Router = router; 
			RouteScanner = routeScanner;

			this.logger = logger; 
			listener = new HttpListener();
			listenerThread = new Thread(ListenAsync); 
		}

		public void Start(string host, int port)
		{
			if (IsDisposed)
				throw new ObjectDisposedException(GetType().FullName);

			if (IsStopping || IsStarting)
				return;

			IsStarting = true;

			try
			{
				var prefix = $@"http://{host}:{port}/";
				logger.LogTrace($"Starting server at {prefix}");
				listener.Prefixes.Add(prefix);
				listener.Start();
				listenerThread.Start();
				logger.LogTrace("Server is started");
			}
			catch (Exception ex)
			{
				logger.LogCritical(ex, "Failed while starting server");
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
				logger.LogTrace("Stopping server");
				listener.Stop();
				listener.Close();
				listenerThread.Join();
				logger.LogTrace("Server is stopped");
			}
			catch (Exception ex)
			{
				logger.LogCritical(ex, "Failed while stopping server");
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

					var httpContext = new HttpContext(context)
					{
						Services = ServiceProvider.CreateScope().ServiceProvider
					};

					ThreadPool.QueueUserWorkItem(ProcessRequestAsync, httpContext, false);
				}
				catch (HttpListenerException ex) when (ex.ErrorCode == 995 && (IsStopping || !IsListening))
				{
					logger.LogWarning(ex, $"{ex.ErrorCode}");
				}
				catch (Exception ex)
				{
					logger.LogCritical(ex, "Failed to process incoming request");
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
