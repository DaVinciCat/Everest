using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using Everest.OpenApi.Filters;
using Everest.Routing;

namespace Everest.OpenApi
{
    public class OpenApiDocumentGenerator
    {
        public ILogger<OpenApiDocumentGenerator> Logger { get; }

        public IList<IOpenApiDocumentFilter> Filters { get; }

        public OpenApiDocumentGenerator(ILogger<OpenApiDocumentGenerator> logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            Filters = new List<IOpenApiDocumentFilter>()
            {
                new RestRouteDocumentFilter(),
                new TagsDocumentFilter(),
                new RequestBodyDocumentFilter(),
                new RequestBodyExampleDocumentFilter(),
                new ResponseDocumentFilter(),
                new ResponseExampleDocumentFilter()
                //new DescriptionOpenApiDocumentFilter(),
                //new ProducesBodyOpenApiDocumentFilter(),
                //new ConsumesParameterOpenApiDocumentFilter()
            };
        }

        public OpenApiDocument Generate(IEnumerable<RouteDescriptor> descriptors, OpenApiInfo info)
        {
            if (descriptors == null)
                throw new ArgumentNullException(nameof(descriptors));

            if (info == null)
                throw new ArgumentNullException(nameof(info));

            var document = new OpenApiDocument
            {
                Info = info,
                Paths = new OpenApiPaths(),
                Components = new OpenApiComponents(),
            };

            Logger.LogTrace($"Generating OpenApi document: {new { Title = info.Title, Version = info.Version }}");

            foreach (var descriptor in descriptors)
            {
                foreach (var filter in Filters)
                {
                    filter.Apply(document, descriptor);
                }
            }

            Logger.LogTrace($"Successfully generated OpenApi document: {new { Title = info.Title, Version = info.Version }}");
            return document;
        }
    }
}
