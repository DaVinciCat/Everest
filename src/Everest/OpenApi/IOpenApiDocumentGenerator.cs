using System.Collections.Generic;
using Everest.Routing;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi
{
    public interface IOpenApiDocumentGenerator
    {
        OpenApiDocument Generate(IEnumerable<RouteDescriptor> descriptor);
    }
}