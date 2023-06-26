using System.Collections.Generic;
using Everest.Http;

namespace Everest.Routing
{
	public interface IRouter
	{
		public RouteDescriptor[] RoutingTable { get; }

		void RegisterRoute(RouteDescriptor descriptor);

		bool TryRoute(HttpContext context);
	}
}
