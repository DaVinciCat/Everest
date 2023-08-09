using System;
using Everest.Services;

namespace Everest.Exceptions
{
	public class ExceptionHandlerConfigurator : ServiceConfigurator<IExceptionHandler>
	{
		public IExceptionHandler Handler => Service;

		public ExceptionHandlerConfigurator(IExceptionHandler handler, IServiceProvider services) 
			: base(handler, services)
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
