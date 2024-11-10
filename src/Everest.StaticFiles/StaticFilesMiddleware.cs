using System;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Middlewares;
using Everest.EndPoints;

namespace Everest.StaticFiles
{
	public class StaticFilesMiddleware : Middleware
	{
		private readonly IStaticFileRequestHandler staticFileRequestHandler;

        private readonly IStaticFilesProvider staticFilesProvider;

		public StaticFilesMiddleware(IStaticFileRequestHandler staticFileRequestHandler, IStaticFilesProvider staticFilesProvider)
		{
			this.staticFileRequestHandler = staticFileRequestHandler ?? throw new ArgumentNullException(nameof(staticFileRequestHandler));
            this.staticFilesProvider = staticFilesProvider ?? throw new ArgumentNullException(nameof(staticFilesProvider));
        }

		public override async Task InvokeAsync(IHttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

            if (context.Response.ResponseSent || context.TryGetEndPoint(out _) || !staticFilesProvider.IsStaticFileRequest(context.Request) || !await staticFileRequestHandler.TryServeStaticFileAsync(context))
            {
                if (HasNext)
                    await Next.InvokeAsync(context);
            }
        }
	}
}
