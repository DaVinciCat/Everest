using System;
using Everest.Common.Configuration;

namespace Everest.Exceptions
{
	public class ExceptionHandlerConfigurator : ServiceConfigurator<ExceptionHandler>
	{
		public ExceptionHandler ExceptionHandler => Service;

		public ExceptionHandlerConfigurator(ExceptionHandler exceptionHandler, IServiceProvider services) 
			: base(exceptionHandler, services)
		{

		}
	}
}
