using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Everest.Common.Logging;
using Everest.Core.Http;

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

        public virtual async Task<bool> Handle(IHttpContext context, CancellationToken token)
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
                        Logger.LogTraceIfEnabled(() => $"{context.TraceIdentifier} - Try to accept WebSocket request: {new { RequestPath = context.Request.Path, RemoteEndPoint = context.Request.RemoteEndPoint }}");
                        
                        var session = await context.WebSockets.AcceptWebSocketAsync();

                        Logger.LogTraceIfEnabled(() => $"{context.TraceIdentifier} - Successfully opened WebSocket session: {new { Id = session.Id, State = session.State, IsLocal = session.IsLocal }}");
                        await ReceiveAsync(session, token);

                        Logger.LogTraceIfEnabled(() => $"{context.TraceIdentifier} - Successfully closed WebSocket session: {new { Is = session.Id, State = session.State, CloseStatus = session.CloseStatus, CloseDescription = session.CloseStatusDescription }}");
                        return true;
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.LogErrorIfEnabled(() => (ex, $"{context.TraceIdentifier} - Failed to accept WebSocket"));
                return false;
            }
            
            return false;
        }
    }
}
