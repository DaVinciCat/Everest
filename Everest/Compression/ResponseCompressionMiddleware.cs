using System;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Middleware;

namespace Everest.Compression
{
    public class ResponseCompressionMiddleware : MiddlewareBase
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

	        if (HasNext)
		        await Next.InvokeAsync(context);

	        if (!context.Response.ResponseSent && context.Request.SupportsContentCompression())
	        {
		        await responseCompressor.TryCompressResponseAsync(context);
	        }
        }
    }
}
