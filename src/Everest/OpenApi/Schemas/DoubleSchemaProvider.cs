using System;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Schemas
{
    internal class DoubleSchemaProvider : IOpenApiSchemaProvider
    {
        public bool TryGetSchema(Type type, out OpenApiSchema schema)
        {
            if (typeof(double) == type)
            {
                schema = new OpenApiSchema { Format = OpenApiDataType.Double.Format, Type = OpenApiDataType.Double.Type };
                return true;
            }

            schema = null;
            return false;
        }
    }
}
