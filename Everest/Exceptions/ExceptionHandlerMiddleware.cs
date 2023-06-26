using System;
using Everest.Http;
using Everest.Middleware;
using Everest.Response;

namespace Everest.Exceptions
{
    public class ExceptionHandlerMiddleware : MiddlewareBase
    {
        private readonly IExceptionHandler handler;

        private readonly IResponseSender sender;

        public ExceptionHandlerMiddleware(IExceptionHandler handler, IResponseSender sender)
        {
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public override void Invoke(HttpContext context)
        {
	        if (context == null) 
		        throw new ArgumentNullException(nameof(context));

	        try
            {
                if (HasNext)
                    Next.Invoke(context);
            }
            catch (Exception ex)
            {
                handler.Handle(context, ex);
                sender.TrySendResponse(context);
            }
        }
    }
}
