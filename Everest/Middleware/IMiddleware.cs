using Everest.Http;

namespace Everest.Middleware
{
	public interface IMiddleware
	{
		void SetNextMiddleware(IMiddleware next);

		void Invoke(HttpContext context);
	}
}
