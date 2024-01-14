using System;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Middlewares;

namespace Everest.Compression
{
    public class ResponseCompressionMiddleware : Middleware
    {
        private readonly IResponseCompressor responseCompressor;

        public ResponseCompressionMiddleware(IResponseCompressor responseCompressor)
        {
            this.responseCompressor = responseCompressor ?? throw new ArgumentNullException(nameof(responseCompressor));
        }

        public override async Task InvokeAsync(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!context.Response.ResponseSent && context.Request.SupportsContentCompression())
            {
                await responseCompressor.TryCompressResponseAsync(context);
            }

            if (HasNext)
                await Next.InvokeAsync(context);
        }
    }
}
