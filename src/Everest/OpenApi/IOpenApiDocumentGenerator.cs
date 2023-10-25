using Everest.Routing;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi
{
    public interface IOpenApiDocumentGenerator
    {
        OpenApiDocument Generate(OpenApiInfo info, RouteDescriptor[] descriptors);
    }
}