using System;
using System.Linq;
using System.Reflection;
using Everest.Collections;
using Everest.Http;
using Everest.Middleware;
using Everest.ResponseCompression;
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
			get => loggerFactory ??= new NullLoggerFactory();
			private set => loggerFactory = value;
		}

		private ILoggerFactory loggerFactory;

		#endregion

		#region RouteScanner

		public IRouteScanner RouteScanner
		{
			get => routeScanner ??= new RouteScanner(RouteSegmentBuilder, LoggerFactory.CreateLogger<RouteScanner>());
			private set => routeScanner = value;
		}

		private IRouteScanner routeScanner;

		#endregion

		#region EndPointResolver 

		public IEndPointResolver EndPointResolver
		{
			get => endPointResolver ??= new EndPointResolver(Routes, RouteSegmentMatcher, LoggerFactory.CreateLogger<EndPointResolver>());
			private set => endPointResolver = value;
		}

		private IEndPointResolver endPointResolver;

		#endregion

		#region EndPointInvoker

		public IEndPointInvoker EndPointInvoker
		{
			get => endPointInvoker ??= new EndPointInvoker(LoggerFactory.CreateLogger<EndPointInvoker>());
			private set => endPointInvoker = value;
		}

		private IEndPointInvoker endPointInvoker;

		#endregion

		#region Services

		public IServiceCollection Services
		{
			get => serviceCollection ??= new ServiceCollection();
			private set => serviceCollection = value;
		}

		private IServiceCollection serviceCollection;

		#endregion

		#region Routes

		public IRouteCollection Routes
		{
			get => routeCollection ??= new RouteCollection();
			private set => routeCollection = value;
		}

		private IRouteCollection routeCollection;

		#endregion

		#region RouteSegmentBuilder 

		public IRouteSegmentBuilder RouteSegmentBuilder
		{
			get => routeSegmentBuilder ??= new RouteSegmentBuilder();
			private set => routeSegmentBuilder = value;
		}

		private IRouteSegmentBuilder routeSegmentBuilder;

		#endregion

		#region RouteSegmentMatcher 

		public IRouteSegmentMatcher RouteSegmentMatcher
		{
			get => routeSegmentMatcher ??= new RouteSegmentMatcher();
			private set => routeSegmentMatcher = value;
		}

		private IRouteSegmentMatcher routeSegmentMatcher;

		#endregion

		#region Compression

		public ICompressionProvider CompressionProvider
		{
			get => compressionProvider ??= new CompressionProvider();
			private set => compressionProvider = value;
		}

		private ICompressionProvider compressionProvider;

		#endregion

		public RestServer Build()
		{
			var server = new RestServer(Services.BuildServiceProvider(), LoggerFactory.CreateLogger<RestServer>());
			server.UseExceptionHandlingMiddleware(LoggerFactory.CreateLogger<ExceptionHandlingMiddleware>());
			server.UseRoutingMiddleware(EndPointResolver);
			server.UseCorsMiddleware();
			server.UseCompressionMiddleware(CompressionProvider);
			server.UseEndPointMiddleware(EndPointInvoker);

			return server;
		}

		public RestServerBuilder WithLoggerFactory(ILoggerFactory factory)
		{
			LoggerFactory = factory;
			return this;
		}

		public RestServerBuilder WithEndPointInvoker(IEndPointInvoker invoker)
		{
			EndPointInvoker = invoker;
			return this;
		}

		public RestServerBuilder WithRoutes(IRouteCollection routes)
		{
			Routes = routes;
			return this;
		}

		public RestServerBuilder WithRouteSegmentBuilder(IRouteSegmentBuilder builder)
		{
			RouteSegmentBuilder = builder;
			return this;
		}

		public RestServerBuilder WithRouteSegmentMatcher(IRouteSegmentMatcher matcher)
		{
			RouteSegmentMatcher = matcher;
			return this;
		}

		public RestServerBuilder WithEndPointResolver(IEndPointResolver resolver)
		{
			EndPointResolver = resolver;
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
				builder.Routes.Add(route);
			}

			return builder;
		}

		public static RestServerBuilder RegisterRoute(this RestServerBuilder builder, string httpMethod, string routePattern, Action<HttpContext> action)
		{
			var route = new Route(httpMethod, routePattern);
			var segment = builder.RouteSegmentBuilder.Build(routePattern);
			var endPoint = new EndPoint(action.Target?.GetType(), action.Method, action);
			var descriptor = new RouteDescriptor(route, segment, endPoint);
			
			builder.Routes.Add(descriptor);
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
