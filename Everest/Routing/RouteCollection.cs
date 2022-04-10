using Everest.Http;
using System;
using System.Collections.Generic;

namespace Everest.Routing
{
	public class RouteCollection
	{
		private readonly Dictionary<string, Dictionary<RouteSegment, Action<HttpContext>>> routeActions = new();

		private readonly RouteSegmentBuilder builder;

		private readonly RouteSegmentMatcher matcher;

		public RouteCollection(RouteSegmentBuilder builder, RouteSegmentMatcher matcher)
		{
			this.builder = builder;
			this.matcher = matcher;
		}

		public void AddRoute(string httpMethod, string pattern, Action<HttpContext> action)
		{
			if (!routeActions.TryGetValue(httpMethod, out var routes))
			{
				routes = new Dictionary<RouteSegment, Action<HttpContext>>();
				routeActions.Add(httpMethod, routes);
			}

			var segment = builder.Build(pattern);
			routes.Add(segment, action);
		}

		public bool TryGetRoute(HttpContext context, out Action<HttpContext> action)
		{
			action = null;

			var httpMethod = context.Request.HttpMethod;
			var url = context.Request.EndPoint;

			if (!routeActions.TryGetValue(httpMethod, out var routes))
				return false;

			foreach (var (key, value) in routes)
			{
				if (matcher.Match(key, url, context.Request.PathParameters))
				{
					action = value;
					return true;
				}
			}

			return false;
		}
	}
}
