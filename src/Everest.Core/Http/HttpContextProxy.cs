using System;
using System.Security.Claims;
using System.Threading;
using Everest.Common.Collections;
using Everest.Core.WebSockets;
using Microsoft.Extensions.Logging;

namespace Everest.Core.Http
{
    public class HttpContextProxy : IHttpContext
    {
        public virtual Guid TraceIdentifier => context.Request.TraceIdentifier;

        public virtual ClaimsPrincipal User => context.User;

        public virtual IHttpRequest Request => context.Request;

        public virtual IHttpResponse Response => context.Response;

        public virtual IWebSocketContext WebSockets => context.WebSockets;

        public virtual IFeatureCollection Features => context.Features;

        public virtual IServiceProvider Services => context.Services;

        public virtual ILoggerFactory LoggerFactory => context.LoggerFactory;

        public virtual CancellationToken CancellationToken => context.CancellationToken;
        
        private readonly IHttpContext context;

        public HttpContextProxy(IHttpContext context)
        {
            this.context = context;
        }
    }
}
