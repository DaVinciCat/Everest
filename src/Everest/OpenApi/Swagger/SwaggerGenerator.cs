using System;
using Everest.Routing;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using System.IO;
using System.Linq;
using System.Reflection;
using Everest.Utils;
using Microsoft.Extensions.Logging;

namespace Everest.OpenApi.Swagger
{
    public class SwaggerGenerator : ISwaggerGenerator
    {
        public ILogger<SwaggerGenerator> Logger { get; }

        public string SwaggerEndPoint { get; set; } = "/api/swagger.json";
        
        public string SwaggerUiPath { get; set; } = "swagger";

        public string PhysicalPath { get; set; } = "public";
        
        private static string SwaggerDistPath = $"{Assembly.GetExecutingAssembly().GetName().Name}.OpenApi.Swagger.Dist";

        private static string SwaggerInitializerFileName = "swagger-initializer.js";

        private readonly IOpenApiDocumentGenerator generator;

        public SwaggerGenerator(IOpenApiDocumentGenerator generator, ILogger<SwaggerGenerator> logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.generator = generator;
        }

        public void Generate(OpenApiInfo info, RouteDescriptor[] routes)
        {
            if (routes == null)
                throw new ArgumentNullException(nameof(routes));

            var endPointFilePath = Path.Combine(PhysicalPath, SwaggerEndPoint.Trim('/').Replace('/', Path.DirectorySeparatorChar));
            var swaggerUiPath = Path.Combine(PhysicalPath, SwaggerUiPath.Trim('/').Replace('/', Path.DirectorySeparatorChar));

            Logger.LogTraceIfEnabled(() => $"Try generate swagger docs: {new { DocumentFilePath = endPointFilePath, SwaggerUiPath = swaggerUiPath }}");

            var fi = new FileInfo(endPointFilePath);
            var di = new DirectoryInfo(swaggerUiPath);

            fi.CreateFile();
            di.CreateDirectory();

            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly
                .GetManifestResourceNames()
                .Where(s => s.StartsWith(SwaggerDistPath))
                .ToArray();

            foreach (var resourceName in resourceNames)
            {
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    var swaggerFileName = resourceName.Substring(SwaggerDistPath.Length + 1);
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
            
            var document = generator.Generate(info, routes);
            var swagger = new FileInfo(Path.Combine(PhysicalPath, endPointFilePath));
            fi.CreateFile();
            using (var sw = new StreamWriter(fi.FullName))
            {
                document.SerializeAsV3(new OpenApiJsonWriter(sw));
                sw.Close();
            }
        }
    }
}
