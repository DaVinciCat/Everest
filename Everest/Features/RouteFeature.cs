using Everest.Routing;

namespace Everest.Features
{
	public interface IRouteFeature
	{
		Route Route { get; }
	}

	public class RouteFeature : IRouteFeature
	{
		public Route Route { get; }

		public RouteFeature(Route route)
		{
			Route = route;
		}
	}
}
