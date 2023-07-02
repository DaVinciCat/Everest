using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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

			TryRouteAsync = async context =>
			{
				foreach (var router in Routers)
				{
					if (await router.TryRouteAsync(context))
						return true;
				}

				await OnRouteNotFoundAsync(context);
				return false;
			};
		}

		void IRouter.RegisterRoute(RouteDescriptor descriptor)
		{
			RegisterRoute(descriptor);
		}

		Task<bool> IRouter.TryRouteAsync(HttpContext context)
		{
			return TryRouteAsync(context);
		}

		public Action<RouteDescriptor> RegisterRoute { get; set; }

		public Func<HttpContext, Task<bool>> TryRouteAsync { get; set; }

		public Func<HttpContext, Task> OnRouteNotFoundAsync { get; set; } = async context =>
		{
			if (context == null) 
				throw new ArgumentNullException(nameof(context));

			context.Response.KeepAlive = false;
			context.Response.StatusCode = HttpStatusCode.NotFound;
			await context.Response.WriteJsonAsync($"Requested route not found: {context.Request.Description}");
		};
	}
}
