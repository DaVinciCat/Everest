using System;

namespace Everest.Annotations
{
	[AttributeUsage(AttributeTargets.Method)]
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
