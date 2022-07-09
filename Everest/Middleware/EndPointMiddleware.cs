using Everest.Http;
using Everest.Routing;

namespace Everest.Middleware
{
	public class EndPointMiddleware : MiddlewareBase
	{
		private readonly IEndPointInvoker invoker;

		public EndPointMiddleware(IEndPointInvoker invoker)
		{
			this.invoker = invoker;
		}

		public override void Invoke(HttpContext context)
		{
			var endPoint = context.GetEndPoint();
			if(endPoint != null)
				invoker.Invoke(context, endPoint);

			if (HasNext)
				Next.Invoke(context);
		}
	}
}
