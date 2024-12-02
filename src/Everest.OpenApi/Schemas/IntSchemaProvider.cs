using System;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Schemas
{
    public class IntSchemaProvider : IOpenApiSchemaProvider
    {
        public bool TryGetSchema(Type type, out OpenApiSchema schema)
        {
            if (typeof(int) == type || typeof(uint) == type)
            {
                schema = new OpenApiSchema { Format = OpenApiDataType.Integer.Format, Type = OpenApiDataType.Integer.Type };
                return true;
            }

            schema = null;
            return false;
        }
    }
}
