using System;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Schemas
{
    public class EnumSchemaProvider : IOpenApiSchemaProvider
    {
        public bool TryGetSchema(Type type, out OpenApiSchema schema)
        {
            if (!type.IsEnum)
            {
                schema = null;
                return false;
            }

            schema = new OpenApiSchema
            {
                Type = OpenApiDataType.Enum.Type
            };

            foreach (var name in Enum.GetNames(type))
            {
                schema.Enum.Add(new OpenApiString(name));
            }

            return true;
        }
    }
}
