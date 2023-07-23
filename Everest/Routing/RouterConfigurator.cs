using Everest.Services;
using System;

namespace Everest.Routing
{
	public class RouterConfigurator : ServiceConfigurator<IRouter>
	{
		public IRouter Router => Service;

		public RouterConfigurator(IRouter router, IServiceProvider services)
			: base(router, services)
		{
			
		}

		public RouterConfigurator RegisterRoute(RouteDescriptor descriptor)
		{
			Router.RegisterRoute(descriptor);
			return this;
		}
	}

	public class DefaultRouterConfigurator : RouterConfigurator
	{
		public new Router Router { get; } 

		public DefaultRouterConfigurator(Router router, IServiceProvider services) 
			: base(router, services)
		{
			Router = router;
		}
	}
}
