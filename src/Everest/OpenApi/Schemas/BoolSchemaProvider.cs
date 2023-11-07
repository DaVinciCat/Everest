using System;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Schemas
{
    public class BoolSchemaProvider : IOpenApiSchemaProvider
    {
        public bool TryGetSchema(Type type, out OpenApiSchema schema)
        {
            if (typeof(bool) == type)
            {
                schema = new OpenApiSchema { Format = OpenApiDataType.Boolean.Format, Type = OpenApiDataType.Boolean.Type };
                return true;
            }

            schema = null;
            return false;
        }
    }
}
