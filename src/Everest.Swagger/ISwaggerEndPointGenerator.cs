using Everest.Common.Logging;
using Everest.Routing;
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
