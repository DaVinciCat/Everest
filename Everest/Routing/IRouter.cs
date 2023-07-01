using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Routing
{
	public interface IRouter
	{
		public RouteDescriptor[] RoutingTable { get; }

		void RegisterRoute(RouteDescriptor descriptor);

		Task<bool> TryRouteAsync(HttpContext context);
	}
}
