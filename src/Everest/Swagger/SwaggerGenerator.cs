using System;
using Everest.Routing;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using System.IO;
using Everest.Utils;
using Microsoft.Extensions.Logging;
using Everest.OpenApi;

namespace Everest.Swagger
{
    public class SwaggerGenerator : ISwaggerGenerator, IHasLogger
    {
        ILogger IHasLogger.Logger => Logger;

        public ILogger<SwaggerGenerator> Logger { get; }

        public string SwaggerEndPoint { get; set; } = "/api/swagger.json";

        public string SwaggerUiPath { get; set; } = "swagger";

        public string PhysicalPath { get; set; } = "public";

        public string SwaggerInitializerFileName { get; set; } = "swagger-initializer.js";

        public string SwaggerUiDistPath { get; set; } = "swagger-ui-dist";

        private readonly IOpenApiDocumentGenerator generator;
        
        public SwaggerGenerator(IOpenApiDocumentGenerator generator, ILogger<SwaggerGenerator> logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.generator = generator;
        }
        
        public void Generate(OpenApiInfo info, RouteDescriptor[] routes)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            if (routes == null)
                throw new ArgumentNullException(nameof(routes));

            var endPointFilePath = Path.Combine(PhysicalPath, SwaggerEndPoint.Trim('/').Replace('/', Path.DirectorySeparatorChar));
            var swaggerUiPath = Path.Combine(PhysicalPath, SwaggerUiPath.Trim('/').Replace('/', Path.DirectorySeparatorChar));

            Logger.LogTraceIfEnabled(() => $"Generating swagger ui: {new { DocumentFilePath = endPointFilePath, SwaggerUiPath = swaggerUiPath }}");

            var fi = new FileInfo(endPointFilePath);
            fi.CreateFile();
            
            var document = generator.Generate(info, routes);
            fi.CreateFile();
            using (var sw = new StreamWriter(fi.FullName))
            {
                document.SerializeAsV3(new OpenApiJsonWriter(sw));
                sw.Close();
            }
            
            var di = new DirectoryInfo(swaggerUiPath);
            di.CreateDirectory();
            foreach (var file in Directory.GetFiles(SwaggerUiDistPath))
            {
                var destFile = Path.Combine(swaggerUiPath, Path.GetFileName(file));
                File.Copy(file, destFile, true); 
            }

            var swaggerInitializerFilePath = Path.Combine(PhysicalPath, SwaggerUiPath, SwaggerInitializerFileName).Trim('/').Replace('/', Path.DirectorySeparatorChar);
            var content = File.ReadAllText(swaggerInitializerFilePath);
            var replace = $"window.location.protocol + '//' + window.location.host + '/{SwaggerEndPoint.Trim('/')}'";
            var url = "\"https://petstore.swagger.io/v2/swagger.json\"";
            content = content.Replace(url, replace);
            File.WriteAllText(swaggerInitializerFilePath, content);
        }
    }
}
