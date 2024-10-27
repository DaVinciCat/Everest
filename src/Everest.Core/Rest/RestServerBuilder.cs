using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Everest.Core.Http;
using Everest.Core.Middlewares;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Everest.Core.Rest
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

			var server = new RestServer(Middleware.ToArray(), Services, loggerFactory);
			server.AddPrefixes(Prefixes.ToArray());

			return server;
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

        public static RestServerBuilder UseMiddleware(this RestServerBuilder builder, Func<IHttpContext, Func<Task>, Task> middleware)
        {
			builder.Middleware.Add(new UseMiddleware(middleware));
            return builder;
        }
    }
}
