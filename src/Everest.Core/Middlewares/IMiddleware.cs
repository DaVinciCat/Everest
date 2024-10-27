using System.Threading.Tasks;
using Everest.Core.Http;

namespace Everest.Core.Middlewares
{
	public interface IMiddleware
	{
		void SetNextMiddleware(IMiddleware next);

		Task InvokeAsync(IHttpContext context);
	}
}
