using System;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Middleware;

namespace Everest.Cors
{
	public class CorsMiddleware : MiddlewareBase
    {
	    private readonly ICorsRequestHandler handler;

        public CorsMiddleware(ICorsRequestHandler handler)
        {
           this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public override async Task InvokeAsync(HttpContext context)
        {
	        if (context == null) 
		        throw new ArgumentNullException(nameof(context));

	        if(await handler.TryHandleCorsRequestAsync(context))
                return;
	        
            if (HasNext)
                await Next.InvokeAsync(context);
        }
    }
}