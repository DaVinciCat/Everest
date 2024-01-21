using System;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Middlewares;

namespace Everest.Compression
{
    public class ResponseCompressionMiddleware : Middleware
    {
        private readonly IResponseCompressorProvider provider;

        public ResponseCompressionMiddleware(IResponseCompressorProvider provider)
        {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public override async Task InvokeAsync(IHttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!context.Response.ResponseSent && context.Request.SupportsContentCompression())
            {
                context = new CompressingHttpContextWrapper(context, provider);
            }

            if (HasNext)
                await Next.InvokeAsync(context);
        }
    }
}
