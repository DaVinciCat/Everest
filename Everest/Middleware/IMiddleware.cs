using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Middleware
{
	public interface IMiddleware
	{
		void SetNextMiddleware(IMiddleware next);

		Task InvokeAsync(HttpContext context);
	}
}
