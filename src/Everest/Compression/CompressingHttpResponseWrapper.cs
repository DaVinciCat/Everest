using System;
using System.IO;
using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Compression
{
    public sealed class CompressingHttpResponseWrapper : HttpResponseWrapper
    {
        private readonly IResponseCompressor compressor;

        private readonly IHttpContext context;

        public CompressingHttpResponseWrapper(IHttpContext context, IResponseCompressor compressor)
            : base(context.Response)
        {
            this.context = context;
            this.compressor = compressor ?? throw new ArgumentNullException(nameof(compressor));
        }

        public override async Task SendResponseAsync(byte[] content, int offset, int count)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));
            
            if(await compressor.TrySendCompressedResponseAsync(context, content, offset, count))
                return;

            await context.Response.SendResponseAsync(content, offset, count);
        }

        public override async Task SendResponseAsync(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (await compressor.TrySendCompressedResponseAsync(context, stream))
                return;

            await context.Response.SendResponseAsync(stream);
        }
    }
}
