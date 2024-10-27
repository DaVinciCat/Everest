using Microsoft.Extensions.DependencyInjection;
using System;
using Everest.OpenApi;
using Microsoft.Extensions.Logging;

namespace Everest.Swagger
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddSwaggerEndPointGenerator(this IServiceCollection services, Func<IServiceProvider, ISwaggerEndPointGenerator> builder)
        {
            services.AddSingleton(builder);
            return services;
        }

        public static IServiceCollection AddSwaggerEndPointGenerator(this IServiceCollection services, Action<SwaggerEndPointGeneratorConfigurator> configurator)
        {
            services.AddSingleton<ISwaggerEndPointGenerator>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var openApiDocumentGenerator = provider.GetRequiredService<IOpenApiDocumentGenerator>();
                var generator = new SwaggerEndPointGenerator(openApiDocumentGenerator, loggerFactory.CreateLogger<SwaggerEndPointGenerator>());
                configurator(new SwaggerEndPointGeneratorConfigurator(generator, provider));

                return generator;
            });

            return services;
        }
    }
}
