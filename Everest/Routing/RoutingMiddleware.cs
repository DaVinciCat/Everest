using System;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Middleware;

namespace Everest.Routing
{
    public class RoutingMiddleware : MiddlewareBase
    {
	    private readonly IRouter router;
        
        public RoutingMiddleware(IRouter router)
        {
            this.router = router ?? throw new ArgumentNullException(nameof(router));
        }

        public override async Task InvokeAsync(HttpContext context)
        {
	        if (context == null) 
		        throw new ArgumentNullException(nameof(context));

	        await router.TryRouteAsync(context);
            
            if (HasNext)
                await Next.InvokeAsync(context);
        }
    }
}
