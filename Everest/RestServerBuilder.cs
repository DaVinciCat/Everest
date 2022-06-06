using System;
using System.Linq;
using System.Reflection;
using Everest.Http;
using Everest.Middleware;
using Everest.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace Everest
{
	public class RestServerBuilder
	{
		#region LoggerFactory

		public ILoggerFactory LoggerFactory
		{
			get => mLoggerFactory ??= new NullLoggerFactory();
			private set => mLoggerFactory = value;
		}

		private ILoggerFactory mLoggerFactory;

		#endregion

		#region Router

		public IRouter Router
		{
			get => mRouter ??= new Router(new RouteSegmentBuilder(), new RouteSegmentParser(), LoggerFactory.CreateLogger<Router>());
			private set => mRouter = value;
		}

		private IRouter mRouter;

		#endregion

		#region RouteScanner

		public IRouteScanner RouteScanner
		{
			get => mRouteScanner ??= new RouteScanner(LoggerFactory.CreateLogger<RouteScanner>());
			private set => mRouteScanner = value;
		}

		private IRouteScanner mRouteScanner;

		#endregion

		#region Services

		public IServiceCollection Services
		{
			get => mServices ??= new ServiceCollection();
			private set => mServices = value;
		}

		private IServiceCollection mServices;

		#endregion

		#region Compression

		public ICompressionProvider CompressionProvider
		{
			get => mCompressionProvider ??= new CompressionProvider();
			private set => mCompressionProvider = value;
		}

		private ICompressionProvider mCompressionProvider;

		#endregion

		public RestServer Build()
		{
			var server = new RestServer(Services.BuildServiceProvider(), LoggerFactory.CreateLogger<RestServer>());
			server.UseExceptionHandlingMiddleware(LoggerFactory.CreateLogger<ExceptionHandlingMiddleware>());
			server.UseRoutingMiddleware(Router);
			server.UseCompressionMiddleware(CompressionProvider);
			server.UseCorsMiddleware();

			return server;
		}

		public RestServerBuilder WithLoggerFactory(ILoggerFactory factory)
		{
			LoggerFactory = factory;
			return this;
		}

		public RestServerBuilder WithRouter(IRouter router)
		{
			Router = router;
			return this;
		}

		public RestServerBuilder WithRouteScanner(IRouteScanner scanner)
		{
			RouteScanner = scanner;
			return this;
		}

		public RestServerBuilder WithServices(IServiceCollection services)
		{
			Services = services;
			return this;
		}

		public RestServerBuilder WithCompressionProvider(ICompressionProvider provider)
		{
			CompressionProvider = provider;
			return this;
		}
	}

	public static class RestServerBuilderExtensions
	{
		public static RestServerBuilder ScanRoutes(this RestServerBuilder builder, Assembly assembly)
		{
			foreach (var route in builder.RouteScanner.Scan(assembly).ToArray())
			{
				builder.Router.RegisterRoute(route);
			}

			return builder;
		}

		public static RestServerBuilder RegisterRoute(this RestServerBuilder builder, string httpMethod, string routePattern, Action<HttpContext> action)
		{
			builder.Router.RegisterRoute(httpMethod, routePattern, action);
			return builder;
		}

		public static RestServerBuilder RegisterTransientService<TService>(this RestServerBuilder builder, Func<TService> factory)
			where TService : class
		{
			builder.Services.AddTransient(_ => factory());
			return builder;
		}

		public static RestServerBuilder RegisterScopedService<TService>(this RestServerBuilder builder, Func<TService> factory)
			where TService : class
		{
			builder.Services.AddScoped(_ => factory());
			return builder;
		}

		public static RestServerBuilder RegisterSingletonService<TService>(this RestServerBuilder builder, Func<TService> factory)
			where TService : class
		{
			builder.Services.AddSingleton(_ => factory());
			return builder;
		}

		public static RestServerBuilder RegisterTransientService<TService, TImplementation>(this RestServerBuilder builder, Func<TImplementation> factory)
			where TImplementation : class, TService
			where TService : class
		{
			builder.Services.AddTransient<TService, TImplementation>(_ => factory());
			return builder;
		}

		public static RestServerBuilder RegisterScopedService<TService, TImplementation>(this RestServerBuilder builder, Func<TImplementation> factory)
			where TImplementation : class, TService
			where TService : class
		{
			builder.Services.AddScoped(_ => factory());
			return builder;
		}

		public static RestServerBuilder RegisterSingletonService<TService, TImplementation>(this RestServerBuilder builder, Func<TImplementation> factory)
			where TImplementation : class, TService
			where TService : class
		{
			 builder.Services.AddSingleton(_ => factory());
			 return builder;
		}

		public static RestServerBuilder UseConsoleLogger(this RestServerBuilder builder)
		{
			var factory = LoggerFactory.Create(config =>
			{
				config.AddSimpleConsole(options =>
				{
					options.SingleLine = true;
					options.ColorBehavior = LoggerColorBehavior.Enabled;
					options.IncludeScopes = false;
					options.TimestampFormat = "hh:mm:ss:ffff ";
				});

				config.SetMinimumLevel(LogLevel.Trace);
			});

			return builder.WithLoggerFactory(factory);
		}
	}
}
