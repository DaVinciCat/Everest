using Everest.Common.Logging;
using Everest.Routing;
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