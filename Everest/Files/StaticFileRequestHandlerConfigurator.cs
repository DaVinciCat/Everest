using System;
using Everest.Services;

namespace Everest.Files
{
	public class StaticFileRequestHandlerConfigurator : ServiceConfigurator<StaticFileRequestHandler>
	{
		public StaticFileRequestHandler Handler => Service;

		public StaticFileRequestHandlerConfigurator(StaticFileRequestHandler handler, IServiceProvider services) 
			: base(handler, services)
		{

		}
	}
}
