using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.WebSockets;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Everest.WebSockets
{
    public class WebSocketSession : WebSocket
    {
        public override WebSocketCloseStatus? CloseStatus => context.WebSocket.CloseStatus;

        public override string CloseStatusDescription => context.WebSocket.CloseStatusDescription;

        public override WebSocketState State => context.WebSocket.State;

        public override string SubProtocol => context.WebSocket.SubProtocol;

        public IPrincipal User => context.User;

        public CookieCollection Cookie => context.CookieCollection;

        public NameValueCollection Headers => context.Headers;

        public string Origin => context.Origin;

        public bool IsAuthenticated => context.IsAuthenticated;

        public Uri RequestUri => context.RequestUri;

        public bool IsSecureConnection => context.IsSecureConnection;

        public bool IsLocal => context.IsLocal;

        public string SecWebSocketKey => context.SecWebSocketKey;

        public string SecWebSocketVersion => context.SecWebSocketVersion;

        public IEnumerable<string> SecWebSocketProtocols  => context.SecWebSocketProtocols;
        
        private readonly HttpListenerWebSocketContext context;
        
        public WebSocketSession(HttpListenerWebSocketContext context)
        {
            this.context = context; 
        }

        public override void Abort()
        {
            context.WebSocket.Abort();
        }

        public override async Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
        {
            await context.WebSocket.CloseAsync(closeStatus, statusDescription, cancellationToken);
        }

        public override async Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
        {
            await context.WebSocket.CloseOutputAsync(closeStatus, statusDescription, cancellationToken);
        }

        public override void Dispose()
        {
            context.WebSocket.Dispose();
        }

        public override async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            return await context.WebSocket.ReceiveAsync(buffer, cancellationToken);
        }

        public override async Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
        {
            await context.WebSocket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);
        }
    }
}
