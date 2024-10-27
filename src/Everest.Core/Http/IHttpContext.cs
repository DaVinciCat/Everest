using System;
using System.Security.Claims;
using System.Threading;
using Everest.Common.Collections;
using Everest.Core.WebSockets;
using Microsoft.Extensions.Logging;

namespace Everest.Core.Http
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
