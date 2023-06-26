using System;
using System.Linq;
using Everest.Http;

namespace Everest.Routing
{
	public class AggregateRouter : IRouter
	{
		public RouteDescriptor[] RoutingTable => Routers.SelectMany(router => router.RoutingTable).ToArray();

		public IRouter[] Routers { get; set; }

		public AggregateRouter()
			: this(new IRouter[] { })
		{

		}

		public AggregateRouter(IRouter[] routers)
		{
			Routers = routers ?? throw new ArgumentNullException(nameof(routers));

			RegisterRoute = descriptor =>
			{
				foreach (var router in Routers)
				{
					router.RegisterRoute(descriptor);
				}
			};

			TryRoute = context =>
			{
				foreach (var router in Routers)
				{
					if (router.TryRoute(context))
						return true;
				}

				OnRouteNotFound(context);
				return false;
			};
		}

		void IRouter.RegisterRoute(RouteDescriptor descriptor)
		{
			RegisterRoute(descriptor);
		}

		bool IRouter.TryRoute(HttpContext context)
		{
			return TryRoute(context);
		}

		public Action<RouteDescriptor> RegisterRoute { get; set; }

		public Func<HttpContext, bool> TryRoute { get; set; }

		public Action<HttpContext> OnRouteNotFound { get; set; } = context =>
		{
			context.Response.Write404NotFound($"Requested route not found: {context.Request.Description}");
		};
	}
}
