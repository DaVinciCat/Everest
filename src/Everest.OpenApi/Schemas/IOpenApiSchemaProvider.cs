using System;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Schemas
{
    public interface IOpenApiSchemaProvider
    {
        bool TryGetSchema(Type type, out OpenApiSchema schema);
    }
}
