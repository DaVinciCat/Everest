using Everest.Http;
using Everest.Routing;
using Everest.Utils;

namespace Everest.Middleware
{
	public class EndPointMiddleware : MiddlewareBase
	{
		private readonly IRouter router;

		public EndPointMiddleware(IRouter router)
		{
			this.router = router;
		}

		public override void Invoke(HttpContext context)
		{
			var route = context.GetRoute();
			if(route != null)
				router.InvokeRoute(context, route);

			if (HasNext)
				Next.Invoke(context);
		}
	}
}
