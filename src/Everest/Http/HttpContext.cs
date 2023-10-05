using System;
using System.Net;
using System.Security.Claims;
using Everest.Collections;
using Everest.WebSockets;
using Microsoft.Extensions.Logging;

namespace Everest.Http
{
    public class HttpContext
    {
	    public Guid TraceIdentifier { get; }

		public ClaimsPrincipal User { get; }

		public HttpRequest Request { get; }

		public HttpResponse Response { get; }

		public WebSocketContext WebSockets { get; }

        public IFeatureCollection Features { get; }

		public IServiceProvider Services { get; }

		public ILoggerFactory LoggerFactory { get; }
		
		public HttpContext(HttpListenerContext context, IFeatureCollection features, IServiceProvider services, ILoggerFactory loggerFactory)
		{
			if (context == null) 
				throw new ArgumentNullException(nameof(context));

			TraceIdentifier = context.Request.RequestTraceIdentifier;
			LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
			Request = new HttpRequest(context, loggerFactory.CreateLogger<HttpRequest>());
			Response = new HttpResponse(context, loggerFactory.CreateLogger<HttpResponse>());
			WebSockets = new WebSocketContext(context);
            Features = features ?? throw new ArgumentNullException(nameof(features));
			Services = services ?? throw new ArgumentNullException(nameof(services));
			User = new ClaimsPrincipal();
		}
	}
}
