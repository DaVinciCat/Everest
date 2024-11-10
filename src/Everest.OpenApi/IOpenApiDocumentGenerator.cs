using Everest.Routing;
using Everest.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi
{
    public interface IOpenApiDocumentGenerator
    {
        OpenApiDocument Generate(OpenApiInfo info, RouteDescriptor[] descriptors);
    }

    public static class HasLoggerExtensions
    {
        public static ILogger GetLogger(this IOpenApiDocumentGenerator instance) => (instance as IHasLogger)?.Logger;
    }
}