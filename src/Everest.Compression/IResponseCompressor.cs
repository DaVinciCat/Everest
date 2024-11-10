using System.IO;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Utils;
using Microsoft.Extensions.Logging;

namespace Everest.Compression
{
    public interface IResponseCompressor
    {
        Task<bool> TrySendCompressedResponseAsync(IHttpContext context, byte[] content, int offset, int count);

        Task<bool> TrySendCompressedResponseAsync(IHttpContext context, Stream stream);
    }

    public static class HasLoggerExtensions
    {
        public static ILogger GetLogger(this IResponseCompressor instance) => (instance as IHasLogger)?.Logger;
    }
}
