using Everest.Http;
using System;
using System.Collections.Generic;

namespace Everest.Routing
{
	public class RouteCollection
	{
		private readonly Dictionary<string, Dictionary<RouteSegment, Action<HttpContext>>> routeActions = new();

		private readonly RouteSegmentBuilder builder;

		private readonly RouteSegmentParser parser;

		public RouteCollection(RouteSegmentBuilder builder, RouteSegmentParser parser)
		{
			this.builder = builder;
			this.parser = parser;
		}

		public void AddRoute(string httpMethod, string pattern, Action<HttpContext> action)
		{
			if (!routeActions.TryGetValue(httpMethod, out var actions))
			{
				actions = new Dictionary<RouteSegment, Action<HttpContext>>();
				routeActions.Add(httpMethod, actions);
			}

			var segment = builder.Build(pattern);
			actions.Add(segment, action);
		}

		public bool TryGetRouteAction(HttpContext context, out RouteAction routeAction)
		{
			routeAction = null;

			var httpMethod = context.Request.HttpMethod;
			var url = context.Request.EndPoint;

			if (!routeActions.TryGetValue(httpMethod, out var routes))
				return false;

			foreach (var (key, value) in routes)
			{
				if (parser.TryParse(key, url, context.Request.PathParameters))
				{
					routeAction = new RouteAction(httpMethod, key, value);
					return true;
				}
			}

			return false;
		}
	}
}
