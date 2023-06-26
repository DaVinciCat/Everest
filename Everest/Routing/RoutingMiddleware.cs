using System;
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

        public override void Invoke(HttpContext context)
        {
	        if (context == null) 
		        throw new ArgumentNullException(nameof(context));

	        router.TryRoute(context);
            
            if (HasNext)
                Next.Invoke(context);
        }
    }
}
