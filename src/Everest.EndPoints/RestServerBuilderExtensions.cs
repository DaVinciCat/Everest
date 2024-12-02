using Everest.Rest;
using Microsoft.Extensions.DependencyInjection;

namespace Everest.EndPoints
{
    public static class RestServerBuilderExtensions
    {
        public static RestServerBuilder UseEndPointMiddleware(this RestServerBuilder builder)
        {
            var invoker = builder.Services.GetRequiredService<IEndPointInvoker>();
            builder.Middleware.Add(new EndPointMiddleware(invoker));
            return builder;
        }
    }
}
