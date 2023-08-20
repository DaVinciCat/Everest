using Everest.Services;
using System;

namespace Everest.Cors
{
	public class CorsRequestHandlerConfigurator : ServiceConfigurator<CorsRequestHandler>
	{
		public CorsRequestHandler Handler => Service;

		public CorsRequestHandlerConfigurator(CorsRequestHandler handler, IServiceProvider services)
			: base(handler, services)
		{

		}

		public CorsRequestHandlerConfigurator AddDefaultCorsPolicy()
		{
			Handler.AddCorsPolicy(CorsPolicy.Default);
			return this;
		}

		public CorsRequestHandlerConfigurator AddCorsPolicy(CorsPolicy policy)
		{
			Handler.AddCorsPolicy(policy);
			return this;
		}
	}
}
