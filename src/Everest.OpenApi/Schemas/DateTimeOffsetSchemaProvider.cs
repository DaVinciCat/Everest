using Microsoft.OpenApi.Models;
using System;

namespace Everest.OpenApi.Schemas
{
    public class DateTimeOffsetSchemaProvider : IOpenApiSchemaProvider
    {
        public bool TryGetSchema(Type type, out OpenApiSchema schema)
        {
            if (typeof(DateTimeOffset) == type)
            {
                schema = new OpenApiSchema { Format = OpenApiDataType.DateTime.Format, Type = OpenApiDataType.DateTime.Type };
                return true;
            }

            schema = null;
            return false;
        }
    }
}
