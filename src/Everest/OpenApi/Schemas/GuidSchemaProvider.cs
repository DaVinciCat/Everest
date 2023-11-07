using System;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Schemas
{
    public class GuidSchemaProvider : IOpenApiSchemaProvider
    {
        public bool TryGetSchema(Type type, out OpenApiSchema schema)
        {
            if (typeof(Guid) == type)
            {
                schema = new OpenApiSchema { Format = OpenApiDataType.Uuid.Format, Type = OpenApiDataType.Uuid.Type };
                return true;
            }

            schema = null;
            return false;
        }
    }
}
