using System;
using System.IO;
using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Compression
{
	public interface IResponseCompressorProvider
    {
        Task<bool> TryGetResponseCompressorAsync(IHttpContext context, byte[] content, out string encoding, out Func<Stream, Stream> compressor);

        Task<bool> TryGetResponseCompressorAsync(IHttpContext context, Stream stream, out string encoding, out Func<Stream, Stream> compressor);
    }
}
