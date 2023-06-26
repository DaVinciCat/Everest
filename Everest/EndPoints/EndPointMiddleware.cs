using System;
using Everest.Http;
using Everest.Middleware;

namespace Everest.EndPoints
{
    public class EndPointMiddleware : MiddlewareBase
    {
        private readonly IEndPointInvoker invoker;

        public EndPointMiddleware(IEndPointInvoker invoker)
        {
            this.invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));
        }

        public override void Invoke(HttpContext context)
        {
	        if (context == null)
		        throw new ArgumentNullException(nameof(context));

            invoker.TryInvokeEndPoint(context);
            
            if (HasNext)
                Next.Invoke(context);
        }
    }
}
