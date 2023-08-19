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

        private readonly IStaticFilesProvider filesProvider;

		public StaticFilesMiddleware(IStaticFileRequestHandler handler, IStaticFilesProvider filesProvider)
		{
			this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
            this.filesProvider = filesProvider ?? throw new ArgumentNullException(nameof(filesProvider));
        }

		public override async Task InvokeAsync(HttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

            if (context.Response.ResponseSent || context.TryGetRouteDescriptor(out _) || !filesProvider.IsStaticFileRequest(context.Request) || !await handler.TryServeStaticFileAsync(context))
            {
                if (HasNext)
                    await Next.InvokeAsync(context);
            }
        }
	}
}
