using Everest.Utils;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Everest.Swagger
{
    public class SwaggerUiGenerator : ISwaggerUiGenerator, IHasLogger
    {
        ILogger IHasLogger.Logger => Logger;

        public ILogger<SwaggerUiGenerator> Logger { get; }

        public string SwaggerInitializerFileName { get; set; } = "swagger-initializer.js";

        public string SwaggerEndPoint { get; set; } = "/api/swagger.json";

        public string SwaggerUiPath { get; set; } = "swagger";

        public string PhysicalPath { get; set; } = "public";

        public SwaggerUiGenerator(ILogger<SwaggerUiGenerator> logger)
        {
            Logger = logger;
        }

        public void Generate()
        {
            var swaggerUiPath = Path.Combine(PhysicalPath, SwaggerUiPath.Trim('/').Replace('/', Path.DirectorySeparatorChar));
            Logger.LogTraceIfEnabled(() => $"Generating swagger ui: {new { SwaggerUiPath = swaggerUiPath }}");

            var swaggerInitializerFilePath = Path.Combine(PhysicalPath, SwaggerUiPath, SwaggerInitializerFileName).Trim('/').Replace('/', Path.DirectorySeparatorChar);
            var content = File.ReadAllText(swaggerInitializerFilePath);
            var replace = $"window.location.protocol + '//' + window.location.host + '/{SwaggerEndPoint.Trim('/')}'";
            var url = "\"https://petstore.swagger.io/v2/swagger.json\"";
            content = content.Replace(url, replace);
            File.WriteAllText(swaggerInitializerFilePath, content);

            Logger.LogTraceIfEnabled(() => $"Successfully generated swagger ui: {new { SwaggerUiPath = swaggerUiPath }}");
        }
    }
}
