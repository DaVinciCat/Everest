using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Everest.Authentication;
using Everest.Compression;
using Everest.Cors;
using Everest.EndPoints;
using Everest.Exceptions;
using Everest.Middleware;
using Everest.Response;
using Everest.Routing;
using Everest.Services;
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

			services.TryAddSingleton<IResponseSender>(provider =>
			{
				var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
				return new ResponseSender(loggerFactory.CreateLogger<ResponseSender>());
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

			var server = new RestServer(Services, Middleware.ToArray(), loggerFactory.CreateLogger<RestServer>());
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

		public static IServiceCollection AddEndPointInvoker(this IServiceCollection services, Func<IServiceProvider, IEndPointInvoker> builder)
		{
			services.AddSingleton(builder);
			return services;
		}

		public static IServiceCollection AddResponseSender(this IServiceCollection services, Func<IServiceProvider, IResponseSender> builder)
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
				var compressor = new ResponseCompressor(loggerFactory.CreateLogger<ResponseCompressor>());
				configurator(new ResponseCompressorConfigurator(compressor, provider));

				return compressor;
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
				var handler = new CorsRequestHandler(loggerFactory.CreateLogger<CorsRequestHandler>());
				configurator(new CorsRequestHandlerConfigurator(handler, provider));

				return handler;
			});

			return services;
		}

		public static IServiceCollection AddExceptionHandler(this IServiceCollection services, Func<IServiceProvider, IExceptionHandler> builder) 
		{
			services.AddSingleton(builder);
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

		public static IServiceCollection AddConsoleLoggerFactory(this IServiceCollection services)
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

			return services.AddSingleton(factory);
		}
	}

	#region Extensions

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

		public static RestServerBuilder UseEndPointMiddleware(this RestServerBuilder builder)
		{
			var invoker = builder.Services.GetRequiredService<IEndPointInvoker>();
			builder.Middleware.Add(new EndPointMiddleware(invoker));
			return builder;
		}

		public static RestServerBuilder UseResponseMiddleware(this RestServerBuilder builder)
		{
			var sender = builder.Services.GetRequiredService<IResponseSender>();
			builder.Middleware.Add(new ResponseMiddleware(sender));
			return builder;
		}

		public static RestServerBuilder UseResponseCompressionMiddleware(this RestServerBuilder builder)
		{
			var compressor = builder.Services.GetRequiredService<IResponseCompressor>();
			builder.Middleware.Add(new ResponseCompressionMiddleware(compressor));
			return builder;
		}

		public static RestServerBuilder UseCorsMiddleware(this RestServerBuilder builder)
		{
			var handler = builder.Services.GetRequiredService<ICorsRequestHandler>();
			builder.Middleware.Add(new CorsMiddleware(handler));
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
			var handler = builder.Services.GetRequiredService<IExceptionHandler>();
			var sender = builder.Services.GetRequiredService<IResponseSender>();

			builder.Middleware.Add(new ExceptionHandlerMiddleware(handler, sender));
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

	#endregion

	#region Authentication

	public class AuthenticatorConfigurator : ServiceConfigurator<IAuthenticator>
	{
		public IAuthenticator Authenticator => Service;

		public AuthenticatorConfigurator(IAuthenticator authenticator, IServiceProvider services)
			: base(authenticator, services)
		{

		}

		public void AddAuthentication(IAuthentication authentication)
		{
			Service.Authentications.Add(authentication);
		}
	}

	public static class AuthenticatorConfiguratorExtensions
	{
		public static AuthenticatorConfigurator AddBasicAuthentication(this AuthenticatorConfigurator configurator, Action<BasicAuthenticationOptions> options = null)
		{
			var configureOptions = new BasicAuthenticationOptions();
			options?.Invoke(configureOptions);

			var loggerFactory = configurator.Services.GetRequiredService<ILoggerFactory>();
			var authentication = new BasicAuthentication(configureOptions, loggerFactory.CreateLogger<BasicAuthentication>());
			configurator.AddAuthentication(authentication);
			return configurator;
		}

		public static AuthenticatorConfigurator AddJwtTokenAuthentication(this AuthenticatorConfigurator configurator, Action<JwtAuthenticationOptions> options = null)
		{
			var configureOptions = new JwtAuthenticationOptions();
			options?.Invoke(configureOptions);

			var loggerFactory = configurator.Services.GetRequiredService<ILoggerFactory>();
			var authentication = new JwtAuthentication(configureOptions, loggerFactory.CreateLogger<JwtAuthentication>());
			configurator.AddAuthentication(authentication);
			return configurator;
		}
	}

	#endregion

	#region Cors

	public class CorsRequestHandlerConfigurator : ServiceConfigurator<ICorsRequestHandler>
	{
		public ICorsRequestHandler Handler => Service;

		public CorsRequestHandlerConfigurator(ICorsRequestHandler service, IServiceProvider services) 
			: base(service, services)
		{

		}

		public CorsRequestHandlerConfigurator AddDefaultCorsPolicy()
		{
			Handler.Policies.Add(CorsPolicy.Default);
			return this;
		}

		public CorsRequestHandlerConfigurator AddCorsPolicy(CorsPolicy policy)
		{
			Handler.Policies.Add(policy);
			return this;
		}
	}

	#endregion

	#region Compression

	public class ResponseCompressorConfigurator : ServiceConfigurator<IResponseCompressor>
	{
		public IResponseCompressor Compressor => Service;

		public ResponseCompressorConfigurator(IResponseCompressor service, IServiceProvider services) 
			: base(service, services)
		{
		}

		public ResponseCompressorConfigurator AddCompressor(string encoding, Func<Stream, Stream> compressor)
		{
			Compressor.Compressors.Add(encoding, compressor);
			return this;
		}
	}

	#endregion

	#region Routing

	public class RouterConfigurator : ServiceConfigurator<Router>
	{
		public Router Router => Service;

		public RouterConfigurator(Router router, IServiceProvider services)
			: base(router, services)
		{

		}
	}

	#endregion
}
