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
            if (!typeof(IEnumerable).IsAssignableFrom(type))
            {
                schema = null;
                return false;
            }

            OpenApiSchema itemSchema = null;

            var args = type.GetGenericArguments();
            if (args.Length == 1)
            {
                var elementType = type.GetGenericArguments()[0];
                itemSchema = getItemSchema(elementType);
            }

            schema = new OpenApiSchema
            {
                Type = OpenApiDataType.Array.Type
            };

            if (itemSchema != null)
            {
                schema.Items = itemSchema;
            }

            return true;
        }
    }
}
