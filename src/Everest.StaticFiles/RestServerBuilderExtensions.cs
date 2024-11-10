using Everest.Rest;
using Microsoft.Extensions.DependencyInjection;

namespace Everest.StaticFiles
{
    public static class RestServerBuilderExtensions
    {
        public static RestServerBuilder UseStaticFilesMiddleware(this RestServerBuilder builder)
        {
            var staticFileRequestHandler = builder.Services.GetRequiredService<IStaticFileRequestHandler>();
            var staticFilesProvider = builder.Services.GetRequiredService<IStaticFilesProvider>();
            builder.Middleware.Add(new StaticFilesMiddleware(staticFileRequestHandler, staticFilesProvider));
            return builder;
        }
    }
}
