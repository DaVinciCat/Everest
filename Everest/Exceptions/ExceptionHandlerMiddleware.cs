using System;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Middleware;

namespace Everest.Exceptions
{
    public class ExceptionHandlerMiddleware : MiddlewareBase
    {
        private readonly IExceptionHandler handler;
        
        public ExceptionHandlerMiddleware(IExceptionHandler handler)
        {
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public override async Task InvokeAsync(HttpContext context)
        {
	        if (context == null) 
		        throw new ArgumentNullException(nameof(context));

	        try
            {
                if (HasNext)
                    await Next.InvokeAsync(context);
            }
            catch (Exception ex)
            {
                await handler.HandleAsync(context, ex);
            }
        }
    }
}
