using System;
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

        public override void Invoke(HttpContext context)
        {
	        if (context == null) 
		        throw new ArgumentNullException(nameof(context));

	        sender.TrySendResponse(context);

            if (HasNext)
                Next.Invoke(context);
        }
    }
}
