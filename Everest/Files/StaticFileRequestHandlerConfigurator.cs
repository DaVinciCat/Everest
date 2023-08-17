using System;
using Everest.Media;
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

		public DefaultStaticFileRequestHandlerConfigurator AddMimeType(string fileExtension, MimeDescriptor descriptor)
		{
			Handler.AddMimeType(fileExtension, descriptor);
			return this;
		}

		public DefaultStaticFileRequestHandlerConfigurator AddMimeType(string fileExtension, string contentType)
		{
			Handler.AddMimeType(fileExtension, contentType);
			return this;
		}

		public DefaultStaticFileRequestHandlerConfigurator AddMimeType(string fileExtension, string contentType, string contentDisposition)
		{
			Handler.AddMimeType(fileExtension, contentType, contentDisposition);
			return this;
		}
	}
}
