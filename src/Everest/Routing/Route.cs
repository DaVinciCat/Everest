using System;

namespace Everest.Routing
{
	public class Route
	{
		public string Description => $"{HttpMethod} {RoutePattern}";

		public string HttpMethod { get; }

		public string RoutePattern { get; }

		public Route(string httpMethod, string routePattern)
		{
			HttpMethod = httpMethod ?? throw new ArgumentNullException(nameof(httpMethod));
			RoutePattern = routePattern ?? throw new ArgumentNullException(nameof(routePattern));
		}
	}
}
