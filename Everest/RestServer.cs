using Everest.Http;
using Everest.Routing;
using System;
using System.Net;
using System.Threading;
using Everest.Log;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Everest
{
	public class RestServer: IDisposable
	{
		public ILogger<RestServer> Logger { get; set; } = DefaultLogger.CreateLogger<RestServer>();

		public bool IsDisposed { get; private set; }

		public bool IsStopping { get; private set; }

		public bool IsStarting { get; private set; }

		public bool IsListening => listener.IsListening;

		public Router Router { get; set; } = new();
		
		public IServiceProvider ServiceProvider { get; set; }

		public IServiceCollection Services { get; set; } = new ServiceCollection();

		private readonly HttpListener listener;

		private readonly Thread listenerThread;
		
		public RestServer()
		{ 
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
				Logger.LogTrace($"Starting server at {prefix}");
				listener.Prefixes.Add(prefix);
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

					var httpContext = new HttpContext(context)
					{
						Services = ServiceProvider.CreateScope().ServiceProvider
					};

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
