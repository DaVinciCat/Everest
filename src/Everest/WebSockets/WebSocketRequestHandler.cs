using Everest.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Everest.WebSockets
{
    public abstract class WebSocketRequestHandler : WebSocketHandler, IWebSocketRequestHandler
    {
        public virtual string Path { get; set; }

        public ILogger Logger { get; }

        public WebSocketRequestHandler(ILogger logger)
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
                        Logger.LogTrace($"{context.TraceIdentifier} - Try to accept WebSocket: {new { Path = Path }}");
                        var socket = await context.WebSockets.AcceptWebSocketAsync();
                        Logger.LogTrace($"{context.TraceIdentifier} - Receiving messages from WebSocket: {new { State = socket.State }}");
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
