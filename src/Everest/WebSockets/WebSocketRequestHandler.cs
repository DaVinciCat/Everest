using Everest.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Everest.WebSockets
{
    public abstract class WebSocketRequestHandler : WebSocketHandler, IWebSocketRequestHandler
    {
        public virtual string Path => "";

        public ILogger Logger { get; }

        protected WebSocketRequestHandler(ILogger logger)
        {
            Logger = logger;
        }

        public virtual async Task<bool> Handle(HttpContext context, CancellationToken token)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            try
            {
                if (Path == context.Request.Path)
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        Logger.LogTrace($"{context.TraceIdentifier} - Try to accept WebSocket: {new { Path = Path, RemoteEndPoint = context.Request.RemoteEndPoint }}");
                        var socket = await context.WebSockets.AcceptWebSocketAsync();
                        Logger.LogTrace($"{context.TraceIdentifier} - Start receiving messages from WebSocket: {new { State = socket.State, IsLocal = socket.IsLocal }}");
                        await ReceiveAsync(socket, token);
                        Logger.LogTrace($"{context.TraceIdentifier} - Done receiving messages from WebSocket: {new { State = socket.State, CloseStatus = socket.CloseStatus, CloseDescription = socket.CloseStatusDescription }}");
                        return true;
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, $"{context.TraceIdentifier} - Failed to accept WebSocket");
                return false;
            }
            
            return false;
        }
    }
}
