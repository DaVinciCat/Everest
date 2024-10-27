using System;
using Everest.Common.Configuration;

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
