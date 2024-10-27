using System.Threading.Tasks;
using Everest.Common.Logging;
using Everest.Core.Http;
using Microsoft.Extensions.Logging;

namespace Everest.StaticFiles
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