using Everest.Utils;
using Microsoft.Extensions.Logging;

namespace Everest.SwaggerUi
{
    public interface ISwaggerUiGenerator
    {
        void Generate();
    }

    public static partial class HasLoggerExtensions
    {
        public static ILogger GetLogger(this ISwaggerUiGenerator instance) => (instance as IHasLogger)?.Logger;
    }
}
