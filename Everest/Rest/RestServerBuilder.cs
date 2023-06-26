using System;
using System.Collections.Generic;
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

		public RestServerBuilder(IServiceCollection services)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));

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
		public static IServiceCollection AddDefaults(this IServiceCollection services)
		{
			services.TryAddSingleton<ILoggerFactory>(new NullLoggerFactory());

			services.TryAddSingleton<IExceptionHandler>(provider =>
			{
				var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
				return new ExceptionHandler(loggerFactory.CreateLogger<ExceptionHandler>());
			});

			services.TryAddSingleton<IRouteSegmentBuilder>(new RouteSegmentBuilder());

			services.TryAddSingleton<IRouteSegmentParser>(new RouteSegmentParser());

			services.TryAddSingleton<IRouteScanner>(provider =>
			{
				var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
				var builder = provider.GetRequiredService<IRouteSegmentBuilder>();
				return new RouteScanner(builder, loggerFactory.CreateLogger<RouteScanner>());
			});
			
			services.TryAddSingleton<IRouter>(provider =>
			{
				var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
				var parser = provider.GetRequiredService<IRouteSegmentParser>();
				var staticRouter = new StaticRouter(loggerFactory.CreateLogger<StaticRouter>()) { OnRouteNotFound = _ => { }};
				var parsingRouter = new ParsingRouter(parser, loggerFactory.CreateLogger<ParsingRouter>()) { OnRouteNotFound = _ => { } };
				var aggregateRouter = new AggregateRouter(new IRouter[] { staticRouter, parsingRouter })
				{
					RegisterRoute = descriptor =>
					{
						if (descriptor.Segment.IsCompletelyStatic())
						{
							staticRouter.RegisterRoute(descriptor);
						}
						else
						{
							parsingRouter.RegisterRoute(descriptor);
						}
					}
				};

				return aggregateRouter;
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

			services.TryAddSingleton<ICorsRequestHandler>(provider =>
			{
				var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
				return new CorsRequestHandler(loggerFactory.CreateLogger<CorsRequestHandler>());
			});

			services.TryAddSingleton<IAuthenticator>(provider =>
			{
				var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
				return new Authenticator(loggerFactory.CreateLogger<Authenticator>());
			});

			return services;
		}

		public static IServiceCollection AddAuthenticator(this IServiceCollection services, IAuthenticator authenticator)
		{
			services.AddSingleton(authenticator);
			return services;
		}

		public static IServiceCollection AddRouter(this IServiceCollection services, IRouter router)
		{
			services.AddSingleton(router);
			return services;
		}
		
		public static IServiceCollection AddRouteSegmentBuilder(this IServiceCollection services, IRouteSegmentBuilder builder)
		{
			services.AddSingleton(builder);
			return services;
		}

		public static IServiceCollection AddRouteSegmentParser(this IServiceCollection services, IRouteSegmentParser parser)
		{
			services.AddSingleton(parser);
			return services;
		}

		public static IServiceCollection AddEndPointInvoker(this IServiceCollection services, IEndPointInvoker invoker)
		{
			services.AddSingleton(invoker);
			return services;
		}

		public static IServiceCollection AddResponseSender(this IServiceCollection services, IResponseSender sender)
		{
			services.AddSingleton(sender);
			return services;
		}

		public static IServiceCollection AddResponseCompressor(this IServiceCollection services, IResponseCompressor compressor)
		{
			services.AddSingleton(compressor);
			return services;
		}

		public static IServiceCollection AddCorsRequestHanler(this IServiceCollection services, ICorsRequestHandler handler)
		{
			services.AddSingleton(handler);
			return services;
		}
		
		public static IServiceCollection AddExceptionHandler(this IServiceCollection services, IExceptionHandler handler)
		{
			services.AddSingleton(handler);
			return services;
		}

		public static IServiceCollection AddRouteScanner(this IServiceCollection services, IRouteScanner scanner)
		{
			services.AddSingleton(scanner);
			return services;
		}

		public static IServiceCollection AddLoggerFactory(this IServiceCollection services, ILoggerFactory factory)
		{
			services.AddSingleton(factory);
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

		public static RestServerBuilder UseAuthenticationMiddleware(this RestServerBuilder builder, Action<AuthenticatorBuilder> factory)
		{
			var authenticator = builder.Services.GetRequiredService<IAuthenticator>();

			var b = new AuthenticatorBuilder(builder.Services);
			factory(b);

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

	public class AuthenticatorBuilder
	{
		public IServiceProvider Services { get; }

		public AuthenticatorBuilder(IServiceProvider services)
		{
			Services = services;
		}

		public void AddAuthentication(IAuthentication authentication)
		{
			var authenticator = Services.GetRequiredService<IAuthenticator>();
			authenticator.Authentications.Add(authentication);
		}
	}

	public static class AuthenticatorBuilderExtensions
	{
		public static AuthenticatorBuilder AddBasicAuthentication(this AuthenticatorBuilder builder, Action<BasicAuthenticationOptions> factory = null)
		{
			var loggerFactory = builder.Services.GetRequiredService<ILoggerFactory>();
			var options = new BasicAuthenticationOptions();
			factory?.Invoke(options);

			var authentication = new BasicAuthentication(options, loggerFactory.CreateLogger<BasicAuthentication>());
			builder.AddAuthentication(authentication);
			return builder;
		}

		public static AuthenticatorBuilder AddJwtTokenAuthentication(this AuthenticatorBuilder builder, Action<JwtAuthenticationOptions> factory = null)
		{
			var loggerFactory = builder.Services.GetRequiredService<ILoggerFactory>();
			var options = new JwtAuthenticationOptions();
			factory?.Invoke(options);

			var authentication = new JwtAuthentication(options, loggerFactory.CreateLogger<JwtAuthentication>());
			builder.AddAuthentication(authentication);
			return builder;
		}
	}
}
