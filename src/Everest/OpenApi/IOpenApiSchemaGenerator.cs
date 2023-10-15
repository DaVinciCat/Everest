using System;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi
{
    public interface IOpenApiSchemaGenerator
    {
        OpenApiSchema GetSchema(Type type);
    }
}