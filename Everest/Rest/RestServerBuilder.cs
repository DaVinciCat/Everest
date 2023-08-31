using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Everest.Authentication;
using Everest.Compression;
using Everest.Cors;
using Everest.EndPoints;
using Everest.Exceptions;
using Everest.Files;
using Everest.Middleware;
using Everest.Mime;
using Everest.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace Everest.Rest
{
	public class RestServerBuilder
	{
		public IList<string> Prefixes { get; } = new List<string>();

		public IList<IMiddleware> Middleware { get; } = new List<IMiddleware>();

		public IServiceProvider Services { get; }

		public RestServerBuilder(IServiceCollection services = null)
		{
			services ??= new ServiceCollection();

			services.TryAddSingleton<ILoggerFactory>(new NullLoggerFactory());

			services.TryAddSingleton<IExceptionHandler>(provider =>
			{
				var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
				return new ExceptionHandler(loggerFactory.CreateLogger<ExceptionHandler>());
			});

			services.TryAddSingleton<IRouteScanner>(provider =>
			{
				var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
				return new RouteScanner(loggerFactory.CreateLogger<RouteScanner>());
			});

			services.TryAddSingleton<IRouter>(provider =>
			{
				var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
				return new Router(loggerFactory.CreateLogger<Router>());
			});

			services.TryAddSingleton<IStaticFilesProvider>(provider =>
			{
				var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
				return new StaticFilesProvider(loggerFactory.CreateLogger<StaticFilesProvider>());
			});

			services.TryAddSingleton<IMimeTypesProvider>(_ => new MimeTypesProvider());

			services.TryAddSingleton<IStaticFileRequestHandler>(provider =>
			{
				var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
				var staticFilesProvider = provider.GetRequiredService<IStaticFilesProvider>();
				var mimeTypesProvider = provider.GetRequiredService<IMimeTypesProvider>();
				return new StaticFileRequestHandler(staticFilesProvider, mimeTypesProvider, loggerFactory.CreateLogger<StaticFileRequestHandler>());
			});

			services.TryAddSingleton<IResponseCompressor>(provider =>
			{
				var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
				return new ResponseCompressor(loggerFactory.CreateLogger<ResponseCompressor>());
			});

			services.TryAddSingleton<IEndPointInvoker>(provider =>
			{
				var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
				return new EndPointInvoker(loggerFactory.CreateLogger<EndPointInvoker>());
			});
			
			Services = services.BuildServiceProvider();
		}

		public RestServer Build()
		{
			var loggerFactory = Services.GetRequiredService<ILoggerFactory>();

			var server = new RestServer(Middleware.ToArray(), Services, loggerFactory);
			server.AddPrefixes(Prefixes.ToArray());

			return server;
		}
	}

	public static class ServicesExtensions
	{
		public static IServiceCollection AddAuthenticator(this IServiceCollection services, Func<IServiceProvider, IAuthenticator> builder)
		{
			if (builder == null) 
				throw new ArgumentNullException(nameof(builder));

			services.AddSingleton(builder);
			return services;
		}

		public static IServiceCollection AddAuthenticator(this IServiceCollection services, Action<AuthenticatorConfigurator> configurator)
		{
			if (configurator == null) 
				throw new ArgumentNullException(nameof(configurator));

			services.AddSingleton<IAuthenticator>(provider =>
			{
				var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
				var authenticator = new Authenticator(loggerFactory.CreateLogger<Authenticator>());
				configurator(new AuthenticatorConfigurator(authenticator, provider));

				return authenticator;
			});

			return services;
		}

		public static IServiceCollection AddRouter(this IServiceCollection services, Func<IServiceProvider, IRouter> builder)
		{
			services.AddSingleton(builder);
			return services;
		}

		public static IServiceCollection AddRouter(this IServiceCollection services, Action<RouterConfigurator> configurator)
		{
			services.AddSingleton<IRouter>(provider =>
			{
				var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
				var router = new Router(loggerFactory.CreateLogger<Router>());
				configurator(new RouterConfigurator(router, provider));

				return router;
			});

			return services;
		}

		public static IServiceCollection AddStaticFileRequestHandler(this IServiceCollection services, Func<IServiceProvider, IStaticFileRequestHandler> builder)
		{
			services.AddSingleton(builder);
			return services;
		}

		public static IServiceCollection AddStaticFileRequestHandler(this IServiceCollection services, Action<StaticFileRequestHandlerConfigurator> configurator)
		{
			services.AddSingleton<IStaticFileRequestHandler>(provider =>
			{
				var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
				var staticFilesProvider = provider.GetRequiredService<IStaticFilesProvider>();
				var mimeTypesProvider = provider.GetRequiredService<IMimeTypesProvider>();
				var staticFileRequestHandler = new StaticFileRequestHandler(staticFilesProvider, mimeTypesProvider, loggerFactory.CreateLogger<StaticFileRequestHandler>());
				configurator(new StaticFileRequestHandlerConfigurator(staticFileRequestHandler, provider));

				return staticFileRequestHandler;
			});

			return services;
		}

		public static IServiceCollection AddStaticFilesProvider(this IServiceCollection services, Func<IServiceProvider, IStaticFilesProvider> builder)
		{
			services.AddSingleton(builder);
			return services;
		}

		public static IServiceCollection AddStaticFilesProvider(this IServiceCollection services, Action<StaticFilesProviderConfigurator> configurator)
		{
			services.AddSingleton<IStaticFilesProvider>(provider =>
			{
				var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
				var staticFilesProvider = new StaticFilesProvider(loggerFactory.CreateLogger<StaticFilesProvider>());
				configurator(new StaticFilesProviderConfigurator(staticFilesProvider, provider));

				return staticFilesProvider;
			});

			return services;
		}

		public static IServiceCollection AddMimeTypesProvider(this IServiceCollection services, Func<IServiceProvider, IMimeTypesProvider> builder)
		{
			services.AddSingleton(builder);
			return services;
		}

		public static IServiceCollection AddMimeTypesProvider(this IServiceCollection services, Action<MimeTypesProviderConfigurator> configurator)
		{
			services.AddSingleton<IMimeTypesProvider>(provider =>
			{
				var mimeTypesProvider = new MimeTypesProvider();
				configurator(new MimeTypesProviderConfigurator(mimeTypesProvider, provider));

				return mimeTypesProvider;
			});

			return services;
		}

		public static IServiceCollection AddEndPointInvoker(this IServiceCollection services, Func<IServiceProvider, IEndPointInvoker> builder)
		{
			services.AddSingleton(builder);
			return services;
		}
		
		public static IServiceCollection AddResponseCompressor(this IServiceCollection services, Func<IServiceProvider, IResponseCompressor> builder)
		{
			services.AddSingleton(builder);
			return services;
		}

		public static IServiceCollection AddResponseCompressor(this IServiceCollection services, Action<ResponseCompressorConfigurator> configurator)
		{
			services.AddSingleton<IResponseCompressor>(provider =>
			{
				var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
				var responseCompressor = new ResponseCompressor(loggerFactory.CreateLogger<ResponseCompressor>());
				configurator(new ResponseCompressorConfigurator(responseCompressor, provider));

				return responseCompressor;
			});

			return services;
		}

		public static IServiceCollection AddCorsRequestHandler(this IServiceCollection services, Func<IServiceProvider, ICorsRequestHandler> builder)
		{
			services.AddSingleton(builder);
			return services;
		}

		public static IServiceCollection AddCorsRequestHandler(this IServiceCollection services, Action<CorsRequestHandlerConfigurator> configurator)
		{
			services.AddSingleton<ICorsRequestHandler>(provider =>
			{
				var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
				var corsRequestHandler = new CorsRequestHandler(loggerFactory.CreateLogger<CorsRequestHandler>());
				configurator(new CorsRequestHandlerConfigurator(corsRequestHandler, provider));

				return corsRequestHandler;
			});

			return services;
		}
		
		public static IServiceCollection AddExceptionHandler(this IServiceCollection services, Func<IServiceProvider, IExceptionHandler> builder) 
		{
			services.AddSingleton(builder);
			return services;
		}

		public static IServiceCollection AddExceptionHandler(this IServiceCollection services, Action<ExceptionHandlerConfigurator> configurator)
		{
			services.AddSingleton<IExceptionHandler>(provider =>
			{
				var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
				var exceptionHandler = new ExceptionHandler(loggerFactory.CreateLogger<ExceptionHandler>());
				configurator(new ExceptionHandlerConfigurator(exceptionHandler, provider));

				return exceptionHandler;
			});

			return services;
		}

		public static IServiceCollection AddRouteScanner(this IServiceCollection services, Func<IServiceProvider, IRouteScanner> builder)
		{
			services.AddSingleton(builder);
			return services;
		}

		public static IServiceCollection AddLoggerFactory(this IServiceCollection services, Func<IServiceProvider, ILoggerFactory> builder)
		{
			services.AddSingleton(builder);
			return services;
		}

		public static IServiceCollection AddConsoleLoggerFactory(this IServiceCollection services, Action<ILoggingBuilder> configurator = null)
		{
			var factory = LoggerFactory.Create(config =>
			{
				config.AddSimpleConsole(opt =>
				{
					opt.SingleLine = true;
					opt.ColorBehavior = LoggerColorBehavior.Enabled;
					opt.IncludeScopes = false;
					opt.TimestampFormat = "hh:mm:ss:ffff ";
				});

				config.SetMinimumLevel(LogLevel.Trace);
				configurator?.Invoke(config);
			});

			return services.AddSingleton(factory);
		}
	}

	public static class RestServerBuilderExtensions
	{
		public static RestServerBuilder UsePrefixes(this RestServerBuilder builder, params string[] prefixes)
		{
			foreach (var prefix in prefixes)
			{
				builder.Prefixes.Add(prefix);
			}

			return builder;
		}

		public static RestServerBuilder UseRoutingMiddleware(this RestServerBuilder builder)
		{
			var router = builder.Services.GetRequiredService<IRouter>();
			builder.Middleware.Add(new RoutingMiddleware(router));
			return builder;
		}

		public static RestServerBuilder UseStaticFilesMiddleware(this RestServerBuilder builder)
		{
			var staticFileRequestHandler = builder.Services.GetRequiredService<IStaticFileRequestHandler>();
            var staticFilesProvider = builder.Services.GetRequiredService<IStaticFilesProvider>();
            builder.Middleware.Add(new StaticFilesMiddleware(staticFileRequestHandler, staticFilesProvider));
			return builder;
		}

		public static RestServerBuilder UseEndPointMiddleware(this RestServerBuilder builder)
		{
			var invoker = builder.Services.GetRequiredService<IEndPointInvoker>();
			builder.Middleware.Add(new EndPointMiddleware(invoker));
			return builder;
		}
		
		public static RestServerBuilder UseResponseCompressionMiddleware(this RestServerBuilder builder)
		{
			var responseCompressor = builder.Services.GetRequiredService<IResponseCompressor>();
			builder.Middleware.Add(new ResponseCompressionMiddleware(responseCompressor));
			return builder;
		}

		public static RestServerBuilder UseCorsMiddleware(this RestServerBuilder builder)
		{
			var corsRequestHandler = builder.Services.GetRequiredService<ICorsRequestHandler>();
			builder.Middleware.Add(new CorsMiddleware(corsRequestHandler));
			return builder;
		}
		
		public static RestServerBuilder UseAuthenticationMiddleware(this RestServerBuilder builder)
		{
			var authenticator = builder.Services.GetRequiredService<IAuthenticator>();
			builder.Middleware.Add(new AuthenticationMiddleware(authenticator));
			return builder;
		}

		public static RestServerBuilder UseExceptionHandlerMiddleware(this RestServerBuilder builder)
		{
			var exceptionHandler = builder.Services.GetRequiredService<IExceptionHandler>();
			builder.Middleware.Add(new ExceptionHandlerMiddleware(exceptionHandler));
			return builder;
		}

		public static RestServerBuilder ScanRoutes(this RestServerBuilder builder, Assembly assembly)
		{
			var scanner = builder.Services.GetRequiredService<IRouteScanner>();
			var router = builder.Services.GetService<IRouter>();

			foreach (var route in scanner.Scan(assembly).ToArray())
			{
				router.RegisterRoute(route);
			}

			return builder;
		}
	}
}
