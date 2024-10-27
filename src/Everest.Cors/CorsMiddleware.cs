using System;
using System.Threading.Tasks;
using Everest.Core.Http;
using Everest.Core.Middlewares;
using Everest.EndPoints;

namespace Everest.Cors
{
	public class CorsMiddleware : Middleware
    {
	    private readonly ICorsRequestHandler corsRequestHandler;

        public CorsMiddleware(ICorsRequestHandler corsRequestHandler)
        {
           this.corsRequestHandler = corsRequestHandler ?? throw new ArgumentNullException(nameof(corsRequestHandler));
        }

        public override async Task InvokeAsync(IHttpContext context)
        {
	        if (context == null) 
		        throw new ArgumentNullException(nameof(context));

	        if (HasNext && context.TryGetEndPoint(out _))
	        {
		        await Next.InvokeAsync(context);
				return;
	        }

	        if (!context.Response.ResponseSent && context.Request.IsCorsPreflightRequest())
	        {
		        await corsRequestHandler.TryHandleCorsRequestAsync(context);
	        }
        }
    }
}