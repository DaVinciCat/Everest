using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Logging;

namespace Everest.Cors
{
    public static class ServicesExtensions
    {
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
    }
}
