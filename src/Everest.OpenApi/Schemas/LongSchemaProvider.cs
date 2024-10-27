using System;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Schemas
{
    public class LongSchemaProvider : IOpenApiSchemaProvider
    {
        public static Type Type => typeof(long);

        public bool TryGetSchema(Type type, out OpenApiSchema schema)
        {
            if (typeof(long) == type || typeof(ulong) == type)
            {
                schema = new OpenApiSchema { Format = OpenApiDataType.Long.Format, Type = OpenApiDataType.Long.Type };
                return true;
            }

            schema = null;
            return false;
        }
    }
}
