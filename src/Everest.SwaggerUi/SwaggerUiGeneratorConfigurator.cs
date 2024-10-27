using System;
using Everest.Common.Configuration;

namespace Everest.SwaggerUi
{
    public class SwaggerUiGeneratorConfigurator : ServiceConfigurator<SwaggerUiGenerator>
    {
        public SwaggerUiGenerator Generator => Service;

        public SwaggerUiGeneratorConfigurator(SwaggerUiGenerator service, IServiceProvider services) 
            : base(service, services)
        {
        }
    }

    public static class SwaggerUiGeneratorConfiguratorExtensions
    {
        public static SwaggerUiGeneratorConfigurator UseSwaggerUiPath(this SwaggerUiGeneratorConfigurator configurator, string swaggerUiPath)
        {
            configurator.Generator.SwaggerUiPath = swaggerUiPath;
            return configurator;
        }

        public static SwaggerUiGeneratorConfigurator UseSwaggerEndPoint(this SwaggerUiGeneratorConfigurator configurator, string swaggerEndPoint)
        {
            configurator.Generator.SwaggerEndPoint = swaggerEndPoint;
            return configurator;
        }

        public static SwaggerUiGeneratorConfigurator UsePhysicalPath(this SwaggerUiGeneratorConfigurator configurator, string physicalPath)
        {
            configurator.Generator.PhysicalPath = physicalPath;
            return configurator;
        }
    }
}
