using System;

namespace Everest.Routing
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class RoutePrefixAttribute : Attribute
	{
		public string RoutePrefix { get; }

		public RoutePrefixAttribute(string routePrefix)
		{
			RoutePrefix = routePrefix ?? throw new ArgumentNullException(nameof(routePrefix));
		}
	}
}
