using Microsoft.OpenApi.Models;
using Everest.Routing;
using Everest.Utils;
using Microsoft.Extensions.Logging;

namespace Everest.OpenApi.Swagger
{
    public interface ISwaggerGenerator
    {
        void Generate(OpenApiInfo info, RouteDescriptor[] routes);
    }

    public static partial class HasLoggerExtensions
    {
        public static ILogger GetLogger(this ISwaggerGenerator instance) => (instance as IHasLogger)?.Logger;
    }
}
