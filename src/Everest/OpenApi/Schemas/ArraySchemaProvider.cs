using System;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Schemas
{
    public class ArraySchemaProvider : IOpenApiSchemaProvider
    {
        private readonly Func<Type, OpenApiSchema> getItemSchema;

        public ArraySchemaProvider(Func<Type, OpenApiSchema> getItemSchema)
        {
            this.getItemSchema = getItemSchema;
        }

        public bool TryGetSchema(Type type, out OpenApiSchema schema)
        {
            if (!typeof(Array).IsAssignableFrom(type))
            {
                schema = null;
                return false;
            }

            OpenApiSchema itemSchema = null;

            var elementType = type.GetElementType();
            if (elementType != null)
            {
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
