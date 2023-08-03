using System;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Middleware;
using Everest.Routing;

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

	        if (HasNext && context.TryGetRouteDescriptor(out _))
	        {
		        await Next.InvokeAsync(context);
				return;
	        }

	        if (context.Request.IsCorsPreflightRequest())
	        {
		        await handler.TryHandleCorsRequestAsync(context);
	        }
        }
    }
}