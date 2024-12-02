using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Logging;

namespace Everest.Routing
{
    public static class ServicesExtensions
    {
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

        public static IServiceCollection AddRouter(this IServiceCollection services)
        {
            services.AddSingleton<IRouter>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                return new Router(loggerFactory.CreateLogger<Router>());
            });

            return services;
        }

        public static IServiceCollection AddRouteScanner(this IServiceCollection services, Func<IServiceProvider, IRouteScanner> builder)
        {
            services.AddSingleton(builder);
            return services;
        }
    }
}
