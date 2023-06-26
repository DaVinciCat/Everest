using System;
using System.Net;
using System.Security.Claims;
using Everest.Collections;

namespace Everest.Http
{
    public class HttpContext
	{
		public Guid Id { get; } = Guid.NewGuid();

		public ClaimsPrincipal User { get; }

		public HttpRequest Request { get; }

		public HttpResponse Response { get; }

		public IFeatureCollection Features { get; }

		public IServiceProvider Services { get; }
		
		public HttpContext(HttpListenerContext context, IFeatureCollection features, IServiceProvider services)
		{
			if (context == null) 
				throw new ArgumentNullException(nameof(context));

			Features = features ?? throw new ArgumentNullException(nameof(features));
			Services = services ?? throw new ArgumentNullException(nameof(services));

			User = new ClaimsPrincipal();
			Request = new HttpRequest(context.Request);
			Response = new HttpResponse(context.Response);
		}
	}
}
