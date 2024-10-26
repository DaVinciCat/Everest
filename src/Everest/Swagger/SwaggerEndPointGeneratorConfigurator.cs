using System;
using Everest.Services;
using Microsoft.OpenApi.Models;

namespace Everest.Swagger
{
    public class SwaggerEndPointGeneratorConfigurator : ServiceConfigurator<SwaggerEndPointGenerator>
    {
        public SwaggerEndPointGenerator Generator => Service;
        
        public SwaggerEndPointGeneratorConfigurator(SwaggerEndPointGenerator generator, IServiceProvider services)
            : base(generator, services)
        {
           
        }
    }
    
    public static class SwaggerEndPointGeneratorConfiguratorExtensions
    {
        public static SwaggerEndPointGeneratorConfigurator UseSwaggerEndPoint(this SwaggerEndPointGeneratorConfigurator configurator, string swaggerEndPoint)
        {
            configurator.Generator.SwaggerEndPoint = swaggerEndPoint;
            return configurator;
        }

        public static SwaggerEndPointGeneratorConfigurator UsePhysicalPath(this SwaggerEndPointGeneratorConfigurator configurator, string physicalPath)
        {
            configurator.Generator.PhysicalPath = physicalPath;
            return configurator;
        }

        public static SwaggerEndPointGeneratorConfigurator UseOpenApiInfo(this SwaggerEndPointGeneratorConfigurator configurator, Action<OpenApiInfo> options)
        {
            options(configurator.Generator.OpenApiInfo);
            return configurator;
        }
    }
}
