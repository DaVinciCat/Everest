using System;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Schemas
{
    public class ByteSchemaProvider : IOpenApiSchemaProvider
    {
        public bool TryGetSchema(Type type, out OpenApiSchema schema)
        {
            if (typeof(byte) == type)
            {
                schema = new OpenApiSchema { Format = OpenApiDataType.Byte.Format, Type = OpenApiDataType.Byte.Type };
                return true;
            }

            schema = null;
            return false;
        }
    }
}
