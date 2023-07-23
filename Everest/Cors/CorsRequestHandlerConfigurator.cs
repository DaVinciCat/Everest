using Everest.Services;
using System;

namespace Everest.Cors
{
	public class CorsRequestHandlerConfigurator : ServiceConfigurator<ICorsRequestHandler>
	{
		public ICorsRequestHandler Handler => Service;

		public CorsRequestHandlerConfigurator(ICorsRequestHandler service, IServiceProvider services)
			: base(service, services)
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

	public class DefaultCorsRequestHandlerConfigurator : CorsRequestHandlerConfigurator
	{
		public new CorsRequestHandler Handler { get; }

		public DefaultCorsRequestHandlerConfigurator(CorsRequestHandler handler, IServiceProvider services) 
			: base(handler, services)
		{
			Handler = handler;
		}
	}
}
