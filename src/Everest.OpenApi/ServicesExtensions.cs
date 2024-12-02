using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Logging;

namespace Everest.OpenApi
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddOpenApiDocumentGenerator(this IServiceCollection services, Func<IServiceProvider, IOpenApiDocumentGenerator> builder)
        {
            services.AddSingleton(builder);
            return services;
        }

        public static IServiceCollection AddOpenApiDocumentGenerator(this IServiceCollection services)
        {
            services.AddSingleton<IOpenApiDocumentGenerator>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                return new OpenApiDocumentGenerator(loggerFactory.CreateLogger<OpenApiDocumentGenerator>());
            });

            return services;
        }

        public static IServiceCollection AddOpenApiDocumentGenerator(this IServiceCollection services, Action<OpenApiDocumentGeneratorConfigurator> configurator)
        {
            services.AddSingleton<IOpenApiDocumentGenerator>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var generator = new OpenApiDocumentGenerator(loggerFactory.CreateLogger<OpenApiDocumentGenerator>());
                configurator(new OpenApiDocumentGeneratorConfigurator(generator, provider));

                return generator;
            });

            return services;
        }
    }
}
