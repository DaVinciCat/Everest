using System;
using System.Threading.Tasks;
using Everest.Core.Http;
using Everest.Core.Middlewares;

namespace Everest.Exceptions
{
    public class ExceptionHandlerMiddleware : Middleware
    {
        private readonly IExceptionHandler exceptionHandler;
        
        public ExceptionHandlerMiddleware(IExceptionHandler exceptionHandler)
        {
            this.exceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
        }

        public override async Task InvokeAsync(IHttpContext context)
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
                await exceptionHandler.HandleExceptionAsync(context, ex);
            }
        }
    }
}
