using System;
using Everest.Common.Configuration;

namespace Everest.Routing
{
	public class RouterConfigurator : ServiceConfigurator<Router>
	{
		public Router Router => Service;

		public RouterConfigurator(Router router, IServiceProvider services)
			: base(router, services)
		{
			
		}

		public RouterConfigurator RegisterRoute(RouteDescriptor descriptor)
		{
			Router.RegisterRoute(descriptor);
			return this;
		}
	}
}
