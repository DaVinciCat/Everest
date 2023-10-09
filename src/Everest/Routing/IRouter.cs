using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Routing
{
	public interface IRouter
	{
		RouteDescriptor[] Routes { get; }

		void RegisterRoute(RouteDescriptor descriptor);

		Task<bool> TryRouteAsync(HttpContext context);
	}
}
