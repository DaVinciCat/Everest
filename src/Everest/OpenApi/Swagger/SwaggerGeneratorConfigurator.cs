using System;
using Everest.Services;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Swagger
{
    public class SwaggerGeneratorConfigurator : ServiceConfigurator<SwaggerGenerator>
    {
        public SwaggerGenerator Generator => Service;

        public OpenApiInfo OpenApiInfo { get; set; } = new OpenApiInfo();

        public SwaggerGeneratorConfigurator(SwaggerGenerator service, IServiceProvider services) 
            : base(service, services)
        {

        }
    }

    public static class SwaggerGeneratorConfiguratorExtensions
    {
        public static SwaggerGeneratorConfigurator UseSwaggerEndPoint(this SwaggerGeneratorConfigurator configurator, string swaggerEndPoint)
        {
            if (!string.IsNullOrEmpty(swaggerEndPoint))
            {
                configurator.Generator.SwaggerEndPoint = swaggerEndPoint;
            }

            return configurator;
        }

        public static SwaggerGeneratorConfigurator UseSwaggerUi(this SwaggerGeneratorConfigurator configurator, string swaggerUiPath = null)
        {
            if (!string.IsNullOrEmpty(swaggerUiPath))
            {
                configurator.Generator.SwaggerUiPath = swaggerUiPath;
            }

            return configurator;
        }

        public static SwaggerGeneratorConfigurator UseOpenApiInfo(this SwaggerGeneratorConfigurator configurator, Action<OpenApiInfo> options)
        {
            options(configurator.OpenApiInfo);
            return configurator;
        }
    }
}
