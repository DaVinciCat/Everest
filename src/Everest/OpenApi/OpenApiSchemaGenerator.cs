using System;
using System.Collections.Generic;
using Everest.OpenApi.Schemas;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi
{
    public class OpenApiSchemaGenerator
    {
        public IList<IOpenApiSchemaProvider> SchemaProviders { get; }

        public OpenApiSchemaGenerator()
        {
            SchemaProviders = new List<IOpenApiSchemaProvider>
            {
                new LongSchemaProvider(),
                new FloatSchemaProvider(),
                new DoubleSchemaProvider(),
                new StringSchemaProvider(),
                new ByteSchemaProvider(),
                new BoolSchemaProvider(),
                new DateTimeSchemaProvider(),
                new IntSchemaProvider(),
                new GuidSchemaProvider(),
                new ArraySchemaProvider(GetSchema),
                new EnumSchemaProvider()
            };
        }
        
        public OpenApiSchema GetSchema(Type type)
        {
            foreach (var provider in SchemaProviders)
            {
                if (provider.TryGetSchema(type, out var schema))
                {
                    return schema;
                }
            }

            throw new KeyNotFoundException($"Unsupported schema type: {type}");
        }
    }
}
