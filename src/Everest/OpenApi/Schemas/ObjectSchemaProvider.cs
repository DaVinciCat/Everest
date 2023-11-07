using Microsoft.OpenApi.Models;
using System;
using System.Collections;

namespace Everest.OpenApi.Schemas
{
    public class ObjectSchemaProvider : IOpenApiSchemaProvider
    {
        private readonly Func<Type, OpenApiSchema> getPropertySchema;

        public ObjectSchemaProvider(Func<Type, OpenApiSchema> getPropertySchema)
        {
            this.getPropertySchema = getPropertySchema;
        }


        public bool TryGetSchema(Type type, out OpenApiSchema schema)
        {
            if (type.IsArray || type.IsPrimitive || type.IsEnum || typeof(IEnumerable).IsAssignableFrom(type))
            {
                schema = null;
                return false;
            }

            schema = new OpenApiSchema
            {
                Type = OpenApiDataType.Object.Type,
                Format = OpenApiDataType.Object.Format
            };

            foreach (var property in type.GetProperties())
            {
                var propertySchema = getPropertySchema(property.PropertyType);
                schema.Properties.Add(property.Name, propertySchema);
            }

            foreach (var property in type.GetFields())
            {
                var fieldSchema = getPropertySchema(property.FieldType);
                schema.Properties.Add(property.Name, fieldSchema);
            }

            return true;
        }
    }
}
