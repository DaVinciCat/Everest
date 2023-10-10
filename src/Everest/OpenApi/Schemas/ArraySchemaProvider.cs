using System;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Schemas
{
    internal class ArraySchemaProvider : IOpenApiSchemaProvider
    {
        private readonly Func<Type, OpenApiSchema> getItemSchema;

        public ArraySchemaProvider(Func<Type, OpenApiSchema> getItemSchema)
        {
            this.getItemSchema = getItemSchema;
        }

        public bool TryGetSchema(Type type, out OpenApiSchema schema)
        {
            if (!type.IsArray)
            {
                schema = null;
                return false;
            }

            var elementType = type.GetElementType();
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
