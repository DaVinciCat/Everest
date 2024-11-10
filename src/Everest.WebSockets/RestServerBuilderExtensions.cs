using Everest.Rest;
using Microsoft.Extensions.DependencyInjection;

namespace Everest.WebSockets
{
    public static class RestServerBuilderExtensions
    {
        public static RestServerBuilder UseWebSocketMiddleware<THandler>(this RestServerBuilder builder)
            where THandler : IWebSocketRequestHandler
        {
            var handler = builder.Services.GetRequiredService<THandler>();
            builder.Middleware.Add(new WebSocketMiddleware<THandler>(handler));
            return builder;
        }
    }
}
