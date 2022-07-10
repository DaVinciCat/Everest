using Everest.Collections;

namespace Everest.Routing
{
	public class RouteData
	{
		public RouteDescriptor RouteDescriptor { get; }

		public ParameterCollection PathParameters { get; }

		public RouteData(RouteDescriptor routeDescriptor, ParameterCollection pathParameters)
		{
			RouteDescriptor = routeDescriptor;
			PathParameters = pathParameters;
		}
	}
}
