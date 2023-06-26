using Everest.Http;
using System;

namespace Everest.Exceptions
{
	public interface IExceptionHandler
	{
		void Handle(HttpContext context, Exception ex);
	}
}
