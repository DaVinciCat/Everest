using System.IO;
using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Compression
{
    public interface IResponseCompressor
    {
        Task<bool> TrySendCompressedResponseAsync(IHttpContext context, byte[] content, int offset, int count);

        Task<bool> TrySendCompressedResponseAsync(IHttpContext context, Stream stream);
    }
}
