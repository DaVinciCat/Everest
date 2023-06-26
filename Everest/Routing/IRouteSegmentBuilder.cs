namespace Everest.Routing
{
	public interface IRouteSegmentBuilder
	{
		RouteSegment Build(string routePattern);
	}
}
