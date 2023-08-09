using System;
using Everest.Services;

namespace Everest.Files
{
	public class StaticFileRequestHandlerConfigurator : ServiceConfigurator<IStaticFileRequestHandler>
	{
		public IStaticFileRequestHandler Handler => Service;

		public StaticFileRequestHandlerConfigurator(IStaticFileRequestHandler handler, IServiceProvider services) 
			: base(handler, services)
		{

		}
	}

	public class DefaultStaticFileRequestHandlerConfigurator : StaticFileRequestHandlerConfigurator
	{
		public new StaticFileRequestHandler Handler { get; }

		public DefaultStaticFileRequestHandlerConfigurator(StaticFileRequestHandler handler, IServiceProvider services)
			: base(handler, services)
		{
			Handler = handler;
		}
	}
}
