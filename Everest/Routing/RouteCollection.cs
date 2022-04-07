using Everest.Http;
using System;
using System.Collections.Generic;

namespace Everest.Routing
{
	public class RouteCollection
	{
		private readonly Dictionary<Route, Action<HttpContext>> routedActions = new Dictionary<Route, Action<HttpContext>>();

		public bool AddRoute(string httpMethod, string path, Action<HttpContext> action)
		{
			var route = new Route(httpMethod, path);
			if (!routedActions.TryGetValue(route, out _))
			{
				routedActions.Add(route, action);
				return true;
			}

			return false;
		}

		public bool TryGetRoute(Route route, out Action<HttpContext> action)
		{
			return routedActions.TryGetValue(route, out action);
		}
	}
}
