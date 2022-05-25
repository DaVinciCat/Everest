using System;
using Everest.Http;
using System.Collections.Generic;
using System.Linq;

namespace Everest.Routing
{
	public class RouteTable
	{
		private readonly Dictionary<string, Dictionary<RouteSegment, Route>> methods = new();

		private readonly RouteSegmentBuilder builder;

		private readonly RouteSegmentParser parser;

		public RouteTable(RouteSegmentBuilder builder, RouteSegmentParser parser)
		{
			this.builder = builder;
			this.parser = parser;
		}

		public void RegisterRoute(string httpMethod, string pattern, Action<HttpContext> action)
		{
			RegisterRoute(new Route(httpMethod, pattern, action));
		}

		public void RegisterRoute(Route route)
		{
			if (!methods.TryGetValue(route.HttpMethod, out var actions))
			{
				actions = new Dictionary<RouteSegment, Route>();
				methods.Add(route.HttpMethod, actions);
			}

			if (actions.Any(o => o.Value.Description == route.Description))
				throw new ArgumentException($"Route {route.Description} is already registered");

			var segment = builder.Build(route.Pattern);
			actions.Add(segment, route);
		}

		public bool ResolveRoute(HttpContext context, out Route route)
		{
			route = null;

			var httpMethod = context.Request.HttpMethod;
			var url = context.Request.EndPoint;

			if (!methods.TryGetValue(httpMethod, out var routes))
				return false;

			foreach (var (key, value) in routes)
			{
				if (parser.TryParse(key, url, context.Request.PathParameters))
				{
					route = value;
					return true;
				}
			}

			return false;
		}
	}
}

