using System;
using System.Collections.Generic;
using Everest.Routing;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi
{
    public class OpenApiDocumentContext
    {
        public OpenApiDocument Document { get; }

        public IOpenApiSchemaGenerator SchemaGenerator { get; }

        public IOpenApiPathParametersGenerator PathParametersGenerator { get; }

        public Func<RouteDescriptor, string> GetOpenApiPathItemKey { get; set; } = descriptor => descriptor.GetOpenApiPathItemKey();

        public Func<RouteDescriptor, OperationType> GetOpenApiOperationType { get; set; } = descriptor => descriptor.GetOpenApiOperationType();

        public IEnumerable<T> GetAttributes<T>(RouteDescriptor descriptor) where T : Attribute => descriptor.GetAttributes<T>();
        
        public OpenApiDocumentContext(OpenApiDocument document, IOpenApiSchemaGenerator schemaGenerator, IOpenApiPathParametersGenerator pathParametersGenerator)
        {
            Document = document;
            SchemaGenerator = schemaGenerator;
            PathParametersGenerator = pathParametersGenerator;
        }
    }
}
