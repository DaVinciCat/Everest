using System;
using System.Security.Claims;
using System.Threading;
using Everest.Collections;
using Everest.WebSockets;
using Microsoft.Extensions.Logging;

namespace Everest.Http
{
    public interface IHttpContext
    {
        Guid TraceIdentifier { get; }

        ClaimsPrincipal User { get; }

        IHttpRequest Request { get; }

        IHttpResponse Response { get; }

        IWebSocketContext WebSockets { get; }

        IFeatureCollection Features { get; }

        IServiceProvider Services { get; }

        ILoggerFactory LoggerFactory { get; }

        CancellationToken CancellationToken { get; }
    }
}
