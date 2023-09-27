using Everest.Services;
using System;

namespace Everest.Cors
{
	public class CorsRequestHandlerConfigurator : ServiceConfigurator<CorsRequestHandler>
	{
		public CorsRequestHandler CorsRequestHandler => Service;

		public CorsRequestHandlerConfigurator(CorsRequestHandler corsRequestHandler, IServiceProvider services)
			: base(corsRequestHandler, services)
		{

		}

		public CorsRequestHandlerConfigurator AddDefaultCorsPolicy()
		{
			CorsRequestHandler.Policies.Add(CorsPolicy.Default);
			return this;
		}

		public CorsRequestHandlerConfigurator AddCorsPolicy(CorsPolicy policy)
		{
			CorsRequestHandler.Policies.Add(policy);
			return this;
		}
	}
}
