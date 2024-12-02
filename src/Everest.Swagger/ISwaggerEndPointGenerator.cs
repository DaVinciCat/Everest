using Everest.Routing;
using Everest.Utils;
using Microsoft.Extensions.Logging;

namespace Everest.Swagger
{
    public interface ISwaggerEndPointGenerator
    {
        void Generate(RouteDescriptor[] routes);
    }

    public static partial class HasLoggerExtensions
    {
        public static ILogger GetLogger(this ISwaggerEndPointGenerator instance) => (instance as IHasLogger)?.Logger;
    }
}
