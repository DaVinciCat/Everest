using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using Everest.OpenApi.Filters;
using Everest.Routing;

namespace Everest.OpenApi
{
    public class OpenApiDocumentGenerator : IOpenApiDocumentGenerator
    {
        public ILogger<OpenApiDocumentGenerator> Logger { get; }

        public IList<IOpenApiDocumentFilter> DocumentFilters { get; }

        public OpenApiSchemaGenerator SchemaGenerator { get; }

        public OpenApiPathParametersGenerator PathParametersGenerator { get; }

        public OpenApiInfo OpenApiInfo { get; set; }

        public OpenApiDocumentGenerator(ILogger<OpenApiDocumentGenerator> logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            DocumentFilters = new List<IOpenApiDocumentFilter>
            {
                new RestRouteDocumentFilter(),
                new OperationDocumentFilter(),
                new TagsDocumentFilter(),
                new RequestBodyDocumentFilter(),
                new RequestBodyExampleDocumentFilter(),
                new ResponseDocumentFilter(),
                new ResponseExampleDocumentFilter(),
                new QueryParameterDocumentFilter(),
                new PathParameterDocumentFilter()
            };

            OpenApiInfo = new OpenApiInfo();
            SchemaGenerator = new OpenApiSchemaGenerator();
            PathParametersGenerator = new OpenApiPathParametersGenerator(SchemaGenerator.GetSchema);
        }

        public OpenApiDocument Generate(IEnumerable<RouteDescriptor> descriptors)
        {
            if (descriptors == null)
                throw new ArgumentNullException(nameof(descriptors));
            
            var document = new OpenApiDocument
            {
                Info = OpenApiInfo,
                Paths = new OpenApiPaths(),
                Components = new OpenApiComponents(),
            };
            
            Logger.LogTrace($"Generating OpenApi document: {new { Title = OpenApiInfo.Title, Version = OpenApiInfo.Version }}");

            var context = new OpenApiDocumentContext(document, SchemaGenerator, PathParametersGenerator);
            foreach (var descriptor in descriptors)
            {
                foreach (var filter in DocumentFilters)
                {
                    filter.Apply(context, descriptor);
                }
            }

            Logger.LogTrace($"Successfully generated OpenApi document: {new { Title = OpenApiInfo.Title, Version = OpenApiInfo.Version }}");
            return document;
        }
    }
}
