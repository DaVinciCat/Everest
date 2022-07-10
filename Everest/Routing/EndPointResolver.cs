using Everest.Collections;
using Everest.Http;
using Microsoft.Extensions.Logging;

namespace Everest.Routing
{
	public interface IEndPointResolver
	{
		bool TryResolve(HttpContext context, out RouteData routeData);
	}

	public class EndPointResolver : IEndPointResolver
	{
		public ILogger<EndPointResolver> Logger { get; }

		private readonly IRouteCollection routes;

		public EndPointResolver(IRouteCollection routes, ILogger<EndPointResolver> logger)
		{
			this.routes = routes;
			Logger = logger;
		}

		public bool TryResolve(HttpContext context, out RouteData routeData)
		{
			Logger.LogTrace($"{context.Id} - Routing request for: {context.Request.Description}");
			if (!routes.TryGetRouteData(context, out routeData))
			{
				Logger.LogWarning($"{context.Id} - Route not found");
				return false;
			}

			Logger.LogTrace($"{context.Id} - Route found from: {routeData.RouteDescriptor.Route.Description} to {routeData.RouteDescriptor.EndPoint.Description}");
			return true;
		}
	}
}
