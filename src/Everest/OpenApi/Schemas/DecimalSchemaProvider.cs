using Microsoft.OpenApi.Models;
using System;

namespace Everest.OpenApi.Schemas
{
    public class DecimalSchemaProvider : IOpenApiSchemaProvider
    {
        public bool TryGetSchema(Type type, out OpenApiSchema schema)
        {
            if (typeof(decimal) == type)
            {
                schema = new OpenApiSchema { Format = OpenApiDataType.Double.Format, Type = OpenApiDataType.Double.Type };
                return true;
            }

            schema = null;
            return false;
        }
    }
}
