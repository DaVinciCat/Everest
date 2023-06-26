using System;
using Everest.EndPoints;

namespace Everest.Routing
{
	public class RouteDescriptor
	{
		public Route Route { get; }

		public RouteSegment Segment { get; }

		public EndPoint EndPoint { get; }
		
		public RouteDescriptor(Route route, RouteSegment segment, EndPoint endPoint)
		{
			Route = route ?? throw new ArgumentNullException(nameof(route));
			Segment = segment ?? throw new ArgumentNullException(nameof(segment));
			EndPoint = endPoint ?? throw new ArgumentNullException(nameof(endPoint));
		}
	}
}
