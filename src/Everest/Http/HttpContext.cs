﻿using System;
using System.Net;
using System.Security.Claims;
using System.Threading;
using Everest.Collections;
using Everest.WebSockets;
using Microsoft.Extensions.Logging;

namespace Everest.Http
{
    public class HttpContext : IHttpContext
    {
	    public Guid TraceIdentifier { get; }

		public ClaimsPrincipal User { get; }

		public IHttpRequest Request { get; }

		public IHttpResponse Response { get; }

		public IWebSocketContext WebSockets { get; }

        public IFeatureCollection Features { get; }

		public IServiceProvider Services { get; }

		public ILoggerFactory LoggerFactory { get; }

		public CancellationToken CancellationToken { get; }
		
		public HttpContext(HttpListenerContext context, IFeatureCollection features, IServiceProvider services, ILoggerFactory loggerFactory, CancellationToken token)
		{
			if (context == null) 
				throw new ArgumentNullException(nameof(context));

			TraceIdentifier = context.Request.RequestTraceIdentifier;
			LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
			Request = new HttpRequest(context.Request, loggerFactory.CreateLogger<HttpRequest>());
			Response = new HttpResponse(context.Response, context.Request, loggerFactory.CreateLogger<HttpResponse>());
			Features = features ?? throw new ArgumentNullException(nameof(features));
			Services = services ?? throw new ArgumentNullException(nameof(services));
			User = new ClaimsPrincipal();
            WebSockets = new WebSocketContext(context, User);
			CancellationToken = token;
        }
	}
}
