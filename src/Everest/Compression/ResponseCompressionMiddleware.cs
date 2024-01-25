using System;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Middlewares;

namespace Everest.Compression
{
    public class ResponseCompressionMiddleware : Middleware
    {
        private readonly IResponseCompressor compressor;

        public ResponseCompressionMiddleware(IResponseCompressor compressor)
        {
            this.compressor = compressor ?? throw new ArgumentNullException(nameof(compressor));
        }

        public override async Task InvokeAsync(IHttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!context.Response.ResponseSent && context.Request.SupportsContentCompression())
            {
                context = new CompressingHttpContextWrapper(context, compressor);
            }

            if (HasNext)
                await Next.InvokeAsync(context);
        }
    }
}
