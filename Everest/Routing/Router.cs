using Everest.Http;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Everest.Collections;
using Microsoft.Extensions.Logging;

namespace Everest.Routing
{
	public interface IRouter
	{
		void RegisterRoute(string httpMethod, string routePattern, Action<HttpContext> action);

		void RegisterRoute(Route route);

		bool TryResolveRoute(HttpContext context, out Route route);

		void InvokeRoute(HttpContext context, Route route);
	}

	public class Router : IRouter
	{
		public ILogger<Router> Logger { get; }

		public IRouteSegmentBuilder RouteBuilder { get; }

		public IRouteSegmentMatcher RouteMatcher { get; }

		private readonly Dictionary<string, Dictionary<RouteSegment, Route>> methods = new();

		public Router(IRouteSegmentBuilder builder, IRouteSegmentMatcher matcher, ILogger<Router> logger)
		{
			RouteBuilder = builder;
			RouteMatcher = matcher;
			Logger = logger;
		}
		
		public void RegisterRoute(string httpMethod, string routePattern, Action<HttpContext> action)
		{
			RegisterRoute(new Route(httpMethod, routePattern, action));
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

			var segment = RouteBuilder.Build(route.Pattern);
			actions.Add(segment, route);

			Logger.LogTrace($"Route registered - {route.Description}");
		}

		public void InvokeRoute(HttpContext context, Route route)
		{
			Logger.LogTrace($"{context.Id} - Routing from: {context.Request.Description}	to: {route.Description}");
			route.Invoke(context);
			Logger.LogTrace($"{context.Id} - Routing complete");
		}

		public bool TryResolveRoute(HttpContext context, out Route route)
		{
			Logger.LogTrace($"{context.Id} - Routing request for: {context.Request.Description}");
			if (!TryResolveRouteImpl(context, out route))
			{
				Logger.LogWarning($"{context.Id} - Route not found");
				return false;
			}

			Logger.LogTrace($"{context.Id} - Route found: {route.Description}");
			return true;
		}

		private bool TryResolveRouteImpl(HttpContext context, out Route route)
		{
			route = null;

			var httpMethod = context.Request.HttpMethod;
			var endPoint = context.Request.EndPoint;

			if (!methods.TryGetValue(httpMethod, out var routes))
				return false;

			foreach (var (key, value) in routes)
			{
				var parameters = new NameValueCollection();
				if (RouteMatcher.TryMatch(key, endPoint, parameters))
				{
					context.Request.PathParameters = new ParameterCollection(parameters);
					route = value;
					return true;
				}
			}

			return false;
		}
	}
}
