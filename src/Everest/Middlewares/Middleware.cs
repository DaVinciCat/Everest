using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Middlewares
{
	public abstract class Middleware : IMiddleware
	{
		public IMiddleware Next;

		protected bool HasNext => Next != null;

		public void SetNextMiddleware(IMiddleware next)
		{
			Next = next;
		}

		public abstract Task InvokeAsync(IHttpContext context);
	}
}
