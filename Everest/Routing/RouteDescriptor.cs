using System;
using Everest.EndPoints;

namespace Everest.Routing
{
	public class RouteDescriptor
	{
		public Route Route { get; }
		
		public EndPoint EndPoint { get; }
		
		public RouteDescriptor(Route route, EndPoint endPoint)
		{
			Route = route ?? throw new ArgumentNullException(nameof(route));
			EndPoint = endPoint ?? throw new ArgumentNullException(nameof(endPoint));
		}
	}
}
