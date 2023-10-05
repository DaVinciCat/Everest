using System;
using System.Net;
using System.Threading.Tasks;

namespace Everest.WebSockets
{
    public class WebSocketContext
    {
        public bool IsWebSocketRequest => context.Request.IsWebSocketRequest;

        private readonly HttpListenerContext context;

        public WebSocketContext(HttpListenerContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<WebSocketSession> AcceptWebSocketAsync()
        {
            return await AcceptWebSocketAsync(null);
        }

        public async Task<WebSocketSession> AcceptWebSocketAsync(string subProtocol)
        {
            var ctx = await context.AcceptWebSocketAsync(subProtocol);
            return new WebSocketSession(ctx);
        }
    }
}
