using System;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Middleware;

namespace Everest.Response
{
    public class ResponseMiddleware : MiddlewareBase
    {
        private readonly IResponseSender sender;

        public ResponseMiddleware(IResponseSender sender)
        {
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public override async Task InvokeAsync(HttpContext context)
        {
	        if (context == null) 
		        throw new ArgumentNullException(nameof(context));

	        await sender.TrySendResponseAsync(context);

            if (HasNext)
                await Next.InvokeAsync(context);
        }
    }
}
