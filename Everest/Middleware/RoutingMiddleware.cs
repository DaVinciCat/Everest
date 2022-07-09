using Everest.Collections;
using Everest.Features;
using Everest.Http;
using Everest.Routing;

namespace Everest.Middleware
{
	public class RoutingMiddleware : MiddlewareBase
	{
		private readonly IEndPointResolver resolver;

		public RoutingMiddleware(IEndPointResolver resolver)
		{
			this.resolver = resolver;
		}

		public override void Invoke(HttpContext context)
		{
			if (!context.Request.IsCorsPreflight())
			{
				if (!resolver.TryResolve(context, out var endPoint))
				{
					context.Response.Write404NotFound($"Requested route not found: {context.Request.Description}.");
					return;
				}

				context.Features.Set<IEndPointFeature>(new EndPointFeature(endPoint));
			}
			
			if (HasNext)
				Next.Invoke(context);
		}
	}
}
