using System;
using System.Collections.Generic;
using System.Linq;
using Everest.Http;
using Everest.Routing;

namespace Everest.Collections
{
	public interface IRouteCollection
	{
		void Add(RouteDescriptor routeDescriptor);

		bool TryGetRouteData(HttpContext context, out RouteData routeData);
	}

	public class RouteCollection : IRouteCollection
	{
		private readonly Dictionary<string, HashSet<RouteDescriptor>> methods = new();

		private readonly IRouteSegmentMatcher matcher;

		public RouteCollection(IRouteSegmentMatcher matcher)
		{
			this.matcher = matcher;
		}

		public void Add(RouteDescriptor routeDescriptor)
		{
			if (!methods.TryGetValue(routeDescriptor.Route.HttpMethod, out var routes))
			{
				routes = new HashSet<RouteDescriptor>();
				methods[routeDescriptor.Route.HttpMethod] = routes;
			}

			if (routes.Any(o => o.Route.Description == routeDescriptor.Route.Description))
				throw new ArgumentException($"Duplicate route: {routeDescriptor.Route.Description}.");

			routes.Add(routeDescriptor);
		}

		public bool TryGetRouteData(HttpContext context, out RouteData routeData)
		{
			routeData = null;

			var httpMethod = context.Request.HttpMethod;
			var endPoint = context.Request.EndPoint;

			if (!methods.TryGetValue(httpMethod, out var descriptors))
				return false;

			foreach (var descriptor in descriptors)
			{
				if (matcher.TryMatch(descriptor.Segment, endPoint, out var parameters))
				{
					routeData = new RouteData(descriptor, new ParameterCollection(parameters));
					return true;
				}
			}

			return false;
		}
	}
}
