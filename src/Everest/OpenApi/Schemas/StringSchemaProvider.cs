using System;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Schemas
{
    public class StringSchemaProvider : IOpenApiSchemaProvider
    {
        public bool TryGetSchema(Type type, out OpenApiSchema schema)
        {
            if (typeof(string) == type)
            {
                schema = new OpenApiSchema { Format = OpenApiDataType.String.Format, Type = OpenApiDataType.String.Type };
                return true;
            }

            schema = null;
            return false;
        }
    }
}
