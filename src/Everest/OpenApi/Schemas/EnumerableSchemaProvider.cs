using System;
using System.Collections;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Schemas
{
    public class EnumerableSchemaProvider : IOpenApiSchemaProvider
    {
        private readonly Func<Type, OpenApiSchema> getItemSchema;

        public EnumerableSchemaProvider(Func<Type, OpenApiSchema> getItemSchema)
        {
            this.getItemSchema = getItemSchema;
        }

        public bool TryGetSchema(Type type, out OpenApiSchema schema)
        {
            if (!type.IsGenericType || !typeof(IEnumerable).IsAssignableFrom(type) || type.GetGenericArguments().Length > 1)
            {
                schema = null;
                return false;
            }

            var elementType = type.GetGenericArguments()[0];
            var itemSchema = getItemSchema(elementType);

            schema = new OpenApiSchema
            {
                Type = OpenApiDataType.Array.Type,
                Items = itemSchema
            };

            return true;
        }
    }
}
