using System;

namespace Everest.Routing
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class RestRouteAttribute : Attribute
	{
		public string HttpMethod { get; }

		public string RoutePattern { get; }

		public RestRouteAttribute(string httpMethod, string routePattern)
		{
			HttpMethod = httpMethod;
			RoutePattern = routePattern;
		}
	}
}
