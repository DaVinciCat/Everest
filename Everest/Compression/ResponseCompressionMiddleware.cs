using System;
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

        public override void Invoke(HttpContext context)
        {
	        if (context == null) 
		        throw new ArgumentNullException(nameof(context));

	        compressor.TryCompressResponse(context);
            
            if (HasNext)
                Next.Invoke(context);
        }
    }
}
