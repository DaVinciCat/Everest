using System;
using Everest.Configuration;

namespace Everest.StaticFiles
{
	public class StaticFileRequestHandlerConfigurator : ServiceConfigurator<StaticFileRequestHandler>
	{
		public StaticFileRequestHandler StaticFileRequestHandler => Service;

		public StaticFileRequestHandlerConfigurator(StaticFileRequestHandler staticFileRequestHandler, IServiceProvider services) 
			: base(staticFileRequestHandler, services)
		{

		}
	}
}
