using Everest.Collections;
using Everest.Features;
using Everest.Http;
using Everest.Routing;

namespace Everest.Middleware
{
	public class RoutingMiddleware : MiddlewareBase
	{
		private readonly IRouter router;

		public RoutingMiddleware(IRouter router)
		{
			this.router = router;
		}

		public override void Invoke(HttpContext context)
		{
			if (!context.Request.IsCorsPreflight())
			{
				if (!router.TryResolveRoute(context, out var route))
				{
					context.Response.Write404NotFound($"Requested route not found: {context.Request.Description}.");
					return;
				}

				context.Features.Set<IRouteFeature>(new RouteFeature(route));
			}
			
			if (HasNext)
				Next.Invoke(context);
		}
	}
}
