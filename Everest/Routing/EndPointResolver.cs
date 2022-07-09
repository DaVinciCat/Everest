using System.Linq;
using Everest.Collections;
using Everest.Http;
using Microsoft.Extensions.Logging;

namespace Everest.Routing
{
	public interface IEndPointResolver
	{
		bool TryResolve(HttpContext context, out EndPoint endPoint);
	}

	public class EndPointResolver : IEndPointResolver
	{
		public ILogger<EndPointResolver> Logger { get; }

		private readonly IRouteCollection routes;

		private readonly IRouteSegmentMatcher matcher;

		public EndPointResolver(IRouteCollection routes, IRouteSegmentMatcher matcher, ILogger<EndPointResolver> logger)
		{
			this.routes = routes;
			this.matcher = matcher;

			Logger = logger;
		}

		public bool TryResolve(HttpContext context, out EndPoint endPoint)
		{
			endPoint = null;

			Logger.LogTrace($"{context.Id} - Routing request for: {context.Request.Description}");
			if (!TryResolveRoute(context, out var descriptor))
			{
				Logger.LogWarning($"{context.Id} - Route not found");
				return false;
			}

			endPoint = descriptor.EndPoint;
			Logger.LogTrace($"{context.Id} - Route found from: {descriptor.Route.Description} to {descriptor.EndPoint.Description}");
			return true;
		}

		private bool TryResolveRoute(HttpContext context, out RouteDescriptor routeDescriptor)
		{
			routeDescriptor = null;

			var httpMethod = context.Request.HttpMethod;
			var endPoint = context.Request.EndPoint;

			foreach (var route in routes.Where(o => o.Route.HttpMethod == httpMethod))
			{
				if (matcher.TryMatch(route.Segment, endPoint, out var parameters))
				{
					context.Request.PathParameters = new ParameterCollection(parameters);
					routeDescriptor = route;
					return true;
				}
			}

			return false;
		}
	}
}
