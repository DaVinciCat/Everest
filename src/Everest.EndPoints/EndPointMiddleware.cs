using System;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Middlewares;

namespace Everest.EndPoints
{
    public class EndPointMiddleware : Middleware
    {
        private readonly IEndPointInvoker endPointInvoker;

        public EndPointMiddleware(IEndPointInvoker endPointInvoker)
        {
            this.endPointInvoker = endPointInvoker ?? throw new ArgumentNullException(nameof(endPointInvoker));
        }

        public override async Task InvokeAsync(IHttpContext context)
        {
	        if (context == null)
		        throw new ArgumentNullException(nameof(context));

            await endPointInvoker.TryInvokeEndPointAsync(context);
            
            if (HasNext)
                await Next.InvokeAsync(context);
        }
    }
}
