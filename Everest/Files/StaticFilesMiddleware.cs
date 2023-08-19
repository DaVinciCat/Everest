using Everest.Http;
using Everest.Middleware;
using Everest.Routing;
using System;
using System.Threading.Tasks;

namespace Everest.Files
{
	public class StaticFilesMiddleware : MiddlewareBase
	{
		private readonly IStaticFileRequestHandler handler;

		public StaticFilesMiddleware(IStaticFileRequestHandler handler)
		{
			this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
		}

		public override async Task InvokeAsync(HttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			if (!context.Response.ResponseSent && 
                !context.Request.Path.EndsWith('/') && 
                !context.TryGetRouteDescriptor(out _) &&
                (context.Request.IsGetMethod() || context.Request.IsHeadMethod()) &&
                await handler.TryServeStaticFileAsync(context))
			{
				return;
			}

			if (HasNext)
				await Next.InvokeAsync(context);
		}
	}
}
