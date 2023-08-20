using Everest.Http;
using System;
using System.Threading.Tasks;

namespace Everest.Exceptions
{
	public interface IExceptionHandler
	{
		Task HandleExceptionAsync(HttpContext context, Exception ex);
	}
}
