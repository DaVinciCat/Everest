using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Middlewares
{
	public interface IMiddleware
	{
		void SetNextMiddleware(IMiddleware next);

		Task InvokeAsync(HttpContext context);
	}
}
