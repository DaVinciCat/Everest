using Everest.Http;
using System;
using System.Threading.Tasks;

namespace Everest.Exceptions
{
	public interface IExceptionHandler
	{
		Task HandleAsync(HttpContext context, Exception ex);
	}
}
