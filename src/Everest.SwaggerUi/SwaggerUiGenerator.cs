using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Everest.Utils;
using Microsoft.Extensions.Logging;

namespace Everest.SwaggerUi
{
    public class SwaggerUiGenerator : ISwaggerUiGenerator, IHasLogger
    {
        ILogger IHasLogger.Logger => Logger;

        public ILogger<SwaggerUiGenerator> Logger { get; }

        public string SwaggerInitializerFileName { get; set; } = "swagger-initializer.js";

        public string SwaggerEndPoint { get; set; } = "/api/swagger.json";

        public string SwaggerUiPath { get; set; } = "swagger";

        public string PhysicalPath { get; set; } = "public";

        public static string SwaggerUiDistPath { get; } = $"{Assembly.GetExecutingAssembly().GetName().Name}.dist";

        public SwaggerUiGenerator(ILogger<SwaggerUiGenerator> logger)
        {
            Logger = logger;
        }

        public void Generate()
        {
            var swaggerUiPath = Path.Combine(PhysicalPath, SwaggerUiPath.Trim('/').Replace('/', Path.DirectorySeparatorChar));
            Logger.LogTraceIfEnabled(() => $"Generating swagger ui: {new { SwaggerUiPath = swaggerUiPath }}");

            var di = new DirectoryInfo(swaggerUiPath);
            di.CreateDirectory();

            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly
                .GetManifestResourceNames()
                .Where(s => s.StartsWith(SwaggerUiDistPath))
                .ToArray();

            foreach (var resourceName in resourceNames)
            {
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    var swaggerFileName = resourceName.Substring(SwaggerUiDistPath.Length + 1);
                    var swaggerFilePath = Path.Combine(swaggerUiPath, swaggerFileName);
                    using (var fs = File.Create(swaggerFilePath))
                    {
                        if (stream == null)
                            throw new InvalidOperationException($"Failed to read resource stream: {resourceName}");

                        stream.CopyTo(fs);
                        fs.Close();
                    }

                    stream.Close();

                    if (swaggerFileName == SwaggerInitializerFileName)
                    {
                        var content = File.ReadAllText(swaggerFilePath);
                        var replace = $"window.location.protocol + '//' + window.location.host + '/{SwaggerEndPoint.Trim('/')}'";
                        var url = "\"https://petstore.swagger.io/v2/swagger.json\"";
                        content = content.Replace(url, replace);
                        File.WriteAllText(swaggerFilePath, content);
                    }
                }
            }

            Logger.LogTraceIfEnabled(() => $"Successfully generated swagger ui: {new { SwaggerUiPath = swaggerUiPath }}");
        }
    }
}
