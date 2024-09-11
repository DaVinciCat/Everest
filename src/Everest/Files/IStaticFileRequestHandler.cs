using System.Threading.Tasks;
using Everest.Http;
using Everest.Utils;
using Microsoft.Extensions.Logging;

namespace Everest.Files
{
    public interface IStaticFileRequestHandler
    {
        Task<bool> TryServeStaticFileAsync(IHttpContext context);
    }

    public static partial class HasLoggerExtensions
    {
        public static ILogger GetLogger(this IStaticFileRequestHandler instance) => (instance as IHasLogger)?.Logger;
    }
}