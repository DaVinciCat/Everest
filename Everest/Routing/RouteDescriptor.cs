namespace Everest.Routing
{
	public class RouteDescriptor
	{
		public Route Route { get; }

		public RouteSegment Segment { get; }

		public EndPoint EndPoint { get; }
		
		public RouteDescriptor(Route route, RouteSegment segment, EndPoint endPoint)
		{
			Route = route;
			Segment = segment;
			EndPoint = endPoint;
		}
	}
}
