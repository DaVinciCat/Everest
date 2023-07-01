using System;
using System.Threading.Tasks;
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

        public override async Task InvokeAsync(HttpContext context)
        {
	        if (context == null)
		        throw new ArgumentNullException(nameof(context));

            await invoker.TryInvokeEndPointAsync(context);
            
            if (HasNext)
                await Next.InvokeAsync(context);
        }
    }
}
