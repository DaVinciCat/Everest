using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Everest.Core.WebSockets
{
    public class WebSocketContext : IWebSocketContext
    {
        public bool IsWebSocketRequest => context.Request.IsWebSocketRequest;

        private readonly HttpListenerContext context;

        private readonly ClaimsPrincipal user;

        public WebSocketContext(HttpListenerContext context, ClaimsPrincipal user)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.user = user ?? throw new ArgumentNullException(nameof(user));
        }

        public async Task<WebSocketSession> AcceptWebSocketAsync()
        {
            return await AcceptWebSocketAsync(null);
        }

        public async Task<WebSocketSession> AcceptWebSocketAsync(string subProtocol)
        {
            var webSocketContext = await context.AcceptWebSocketAsync(subProtocol);
            return new WebSocketSession(webSocketContext, context.Request, user);
        }
    }
}
