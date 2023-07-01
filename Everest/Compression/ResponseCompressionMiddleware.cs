using System;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Middleware;

namespace Everest.Compression
{
    public class ResponseCompressionMiddleware : MiddlewareBase
    {
        private readonly IResponseCompressor compressor;

        public ResponseCompressionMiddleware(IResponseCompressor compressor)
        {
            this.compressor = compressor ?? throw new ArgumentNullException(nameof(compressor));
        }

        public override async Task InvokeAsync(HttpContext context)
        {
	        if (context == null) 
		        throw new ArgumentNullException(nameof(context));

	        await compressor.TryCompressResponseAsync(context);
            
            if (HasNext)
                await Next.InvokeAsync(context);
        }
    }
}
