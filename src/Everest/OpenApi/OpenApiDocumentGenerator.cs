using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using Everest.OpenApi.Filters;
using Everest.Routing;
using Everest.Utils;

namespace Everest.OpenApi
{
    public class OpenApiDocumentGenerator : IOpenApiDocumentGenerator
    {
        public ILogger<OpenApiDocumentGenerator> Logger { get; }

        public IList<IOpenApiDocumentFilter> DocumentFilters { get; }

        public OpenApiSchemaGenerator SchemaGenerator { get; }

        public OpenApiPathParametersGenerator PathParametersGenerator { get; }

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

            SchemaGenerator = new OpenApiSchemaGenerator();
            PathParametersGenerator = new OpenApiPathParametersGenerator(SchemaGenerator.GetSchema);
        }

        public OpenApiDocument Generate(OpenApiInfo info, RouteDescriptor[] descriptors)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            if (descriptors == null)
                throw new ArgumentNullException(nameof(descriptors));
            
            var document = new OpenApiDocument
            {
                Info = info,
                Paths = new OpenApiPaths(),
                Components = new OpenApiComponents(),
            };

            Logger.LogTraceIfEnabled(() => $"Generating OpenApi document: {new { Title = info.Title, Version = info.Version }}");

            var context = new OpenApiDocumentContext(document, SchemaGenerator, PathParametersGenerator);
            foreach (var descriptor in descriptors)
            {
                foreach (var filter in DocumentFilters)
                {
                    filter.Apply(context, descriptor);
                }
            }

            Logger.LogTraceIfEnabled(() => $"Successfully generated OpenApi document: {new { Title = info.Title, Version = info.Version }}");
            return document;
        }
    }
}
