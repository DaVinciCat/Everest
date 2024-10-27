using System;
using System.Threading.Tasks;
using Everest.Core.Http;
using Everest.Core.Middlewares;

namespace Everest.Routing
{
    public class RoutingMiddleware : Middleware
    {
	    private readonly IRouter router;
        
        public RoutingMiddleware(IRouter router)
        {
            this.router = router ?? throw new ArgumentNullException(nameof(router));
        }

        public override async Task InvokeAsync(IHttpContext context)
        {
	        if (context == null) 
		        throw new ArgumentNullException(nameof(context));

	        await router.TryRouteAsync(context);
            
            if (HasNext)
                await Next.InvokeAsync(context);
        }
    }
}
