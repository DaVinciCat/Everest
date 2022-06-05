using Everest.Http;
using Everest.Routing;
using Everest.Utils;

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
			if (!router.TryRoute(context))
			{
				context.Response.Write404NotFound($"Requested route not found: {context.Request.Description}.");
				return;
			}
			
			if (HasNext)
				Next.Invoke(context);
		}
	}
}
