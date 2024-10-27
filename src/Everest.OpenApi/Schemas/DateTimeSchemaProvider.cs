using System;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Schemas
{
    public class DateTimeSchemaProvider : IOpenApiSchemaProvider
    {
        public bool TryGetSchema(Type type, out OpenApiSchema schema)
        {
            if (typeof(DateTime) == type)
            {
                schema = new OpenApiSchema { Format = OpenApiDataType.DateTime.Format, Type = OpenApiDataType.DateTime.Type };
                return true;
            }

            schema = null;
            return false;
        }
    }
}
