using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Schemas
{
    public class DictionarySchemaProvider : IOpenApiSchemaProvider
    {
        private readonly Func<Type, OpenApiSchema> getItemSchema;

        public DictionarySchemaProvider(Func<Type, OpenApiSchema> getItemSchema)
        {
            this.getItemSchema = getItemSchema;
        }

        public bool TryGetSchema(Type type, out OpenApiSchema schema)
        {
            if (!type.IsGenericType || (!type.GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>)) && !type.GetGenericTypeDefinition().IsAssignableFrom(typeof(IDictionary<,>))))
            {
                schema = null;
                return false;
            }

            var keyType = type.GetGenericArguments()[0];
            if (typeof(string) != keyType && !keyType.IsEnum)
                throw new ArgumentException($"Unsupported dictionary key type: {keyType}. Schema supports keys of type {typeof(string)} or {typeof(Enum)} only");

            var valueType = type.GetGenericArguments()[1];
            var valueSchema = getItemSchema(valueType);

            schema = new OpenApiSchema
            {
                Type = OpenApiDataType.Object.Type,
                Format = OpenApiDataType.Object.Format
            };

            schema.AdditionalPropertiesAllowed = true;
            schema.AdditionalProperties = valueSchema;
            
            return true;
        }
    }
}
