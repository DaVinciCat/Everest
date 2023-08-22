using System;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Middleware;

namespace Everest.EndPoints
{
    public class EndPointMiddleware : MiddlewareBase
    {
        private readonly IEndPointInvoker endPointInvoker;

        public EndPointMiddleware(IEndPointInvoker endPointInvoker)
        {
            this.endPointInvoker = endPointInvoker ?? throw new ArgumentNullException(nameof(endPointInvoker));
        }

        public override async Task InvokeAsync(HttpContext context)
        {
	        if (context == null)
		        throw new ArgumentNullException(nameof(context));

            await endPointInvoker.TryInvokeEndPointAsync(context);
            
            if (HasNext)
                await Next.InvokeAsync(context);
        }
    }
}
