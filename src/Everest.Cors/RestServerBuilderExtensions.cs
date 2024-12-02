using Everest.Rest;
using Microsoft.Extensions.DependencyInjection;

namespace Everest.Cors
{
    public static class RestServerBuilderExtensions
    {
        public static RestServerBuilder UseCorsMiddleware(this RestServerBuilder builder)
        {
            var corsRequestHandler = builder.Services.GetRequiredService<ICorsRequestHandler>();
            builder.Middleware.Add(new CorsMiddleware(corsRequestHandler));
            return builder;
        }
    }
}
