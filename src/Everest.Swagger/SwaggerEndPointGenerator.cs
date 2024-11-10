using System;
using Everest.Routing;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using System.IO;
using Microsoft.Extensions.Logging;
using Everest.OpenApi;
using Everest.Utils;

namespace Everest.Swagger
{
    public class SwaggerEndPointGenerator : ISwaggerEndPointGenerator, IHasLogger
    {
        ILogger IHasLogger.Logger => Logger;

        public ILogger<SwaggerEndPointGenerator> Logger { get; }

        public string SwaggerEndPoint { get; set; } = "/api/swagger.json";
        
        public string PhysicalPath { get; set; } = "public";

        public OpenApiInfo OpenApiInfo { get; } = new OpenApiInfo();


        private readonly IOpenApiDocumentGenerator generator;
        
        public SwaggerEndPointGenerator(IOpenApiDocumentGenerator generator, ILogger<SwaggerEndPointGenerator> logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.generator = generator;
        }
        
        public void Generate(RouteDescriptor[] routes)
        {
            if (routes == null)
                throw new ArgumentNullException(nameof(routes));

            var swaggerEndPointPath = Path.Combine(PhysicalPath, SwaggerEndPoint.Trim('/').Replace('/', Path.DirectorySeparatorChar));
            
            Logger.LogTraceIfEnabled(() => $"Generating swagger endpoint: {new { SwaggerEndPoint = swaggerEndPointPath }}");

            var fi = new FileInfo(swaggerEndPointPath);
            fi.CreateFile();
            
            var document = generator.Generate(OpenApiInfo, routes);
            fi.CreateFile();
            using (var sw = new StreamWriter(fi.FullName))
            {
                document.SerializeAsV3(new OpenApiJsonWriter(sw));
                sw.Close();
            }

            Logger.LogTraceIfEnabled(() => $"Successfully generated swagger endpoint: {new { SwaggerEndPoint = swaggerEndPointPath }}");
        }
    }
}
