using System;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Schemas
{
    public class FloatSchemaProvider : IOpenApiSchemaProvider
    {
        public bool TryGetSchema(Type type, out OpenApiSchema schema)
        {
            if (typeof(float) == type)
            {
                schema = new OpenApiSchema { Format = OpenApiDataType.Float.Format, Type = OpenApiDataType.Float.Type };
                return true;
            }

            schema = null;
            return false;
        }
    }
}
