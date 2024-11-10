using System.Linq;
using System.Reflection;
using Everest.Rest;
using Microsoft.Extensions.DependencyInjection;

namespace Everest.Routing
{
    public static class RestServerBuilderExtensions
    {
        public static RestServerBuilder UseRoutingMiddleware(this RestServerBuilder builder)
        {
            var router = builder.Services.GetRequiredService<IRouter>();
            builder.Middleware.Add(new RoutingMiddleware(router));
            return builder;
        }

        public static RestServerBuilder ScanRoutes(this RestServerBuilder builder, Assembly assembly)
        {
            var scanner = builder.Services.GetRequiredService<IRouteScanner>();
            var router = builder.Services.GetRequiredService<IRouter>();

            foreach (var route in scanner.Scan(assembly).ToArray())
            {
                router.RegisterRoute(route);
            }

            return builder;
        }
    }
}
