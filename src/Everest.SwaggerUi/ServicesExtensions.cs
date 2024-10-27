using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Logging;

namespace Everest.SwaggerUi
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddSwaggerUiGenerator(this IServiceCollection services, Func<IServiceProvider, ISwaggerUiGenerator> builder)
        {
            services.AddSingleton(builder);
            return services;
        }

        public static IServiceCollection AddSwaggerUiGenerator(this IServiceCollection services, Action<SwaggerUiGeneratorConfigurator> configurator)
        {
            services.AddSingleton<ISwaggerUiGenerator>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var generator = new SwaggerUiGenerator(loggerFactory.CreateLogger<SwaggerUiGenerator>());
                configurator(new SwaggerUiGeneratorConfigurator(generator, provider));

                return generator;
            });

            return services;
        }
    }
}
