using System;
using Everest.Services;

namespace Everest.Exceptions
{
	public class ExceptionHandlerConfigurator : ServiceConfigurator<IExceptionHandler>
	{
		public IExceptionHandler Handler => Service;

		public ExceptionHandlerConfigurator(ExceptionHandler service, IServiceProvider services) 
			: base(service, services)
		{

		}
	}

	public class DefaultExceptionHandlerConfigurator : ExceptionHandlerConfigurator
	{
		public new ExceptionHandler Handler { get; }

		public DefaultExceptionHandlerConfigurator(ExceptionHandler handler, IServiceProvider services) 
			: base(handler, services)
		{
			Handler = handler;
		}
	}
}
