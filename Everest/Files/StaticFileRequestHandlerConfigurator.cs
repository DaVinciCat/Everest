using System;
using Everest.Services;

namespace Everest.Files
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
