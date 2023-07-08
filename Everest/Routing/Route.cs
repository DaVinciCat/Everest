using System;

namespace Everest.Routing
{
	public class Route
	{
		public string Description => $"{HttpMethod} {RoutePath}";

		public string HttpMethod { get; }

		public string RoutePath { get; }

		public Route(string httpMethod, string routePath)
		{
			HttpMethod = httpMethod ?? throw new ArgumentNullException(nameof(httpMethod));
			RoutePath = routePath ?? throw new ArgumentNullException(nameof(routePath));
		}
	}
}
