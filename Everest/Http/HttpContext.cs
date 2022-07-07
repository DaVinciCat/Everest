using System;
using Everest.Features;
using Everest.Routing;
using Everest.Utils;

namespace Everest.Http
{
	public class HttpContext
	{
		public Guid Id { get; } = Guid.NewGuid();

		public HttpRequest Request { get; }

		public HttpResponse Response { get; }

		public IFeatureCollection Features { get; }

		public IServiceProvider Services { get; }
		
		public HttpContext(HttpRequest request, HttpResponse response, IFeatureCollection features, IServiceProvider services)
		{
			Request = request;
			Response = response;
			Features = features;
			Services = services;
		}
	}

	public static class HttpContextExtensions
	{
		public static Route GetRoute(this HttpContext context)
		{
			return context.Features.Get<IRouteFeature>()?.Route;
		}

		public static bool IsCorsPreflight(this HttpContext context)
		{
			return context.Request.HttpMethod == "OPTIONS" &&
			       context.Request.Headers["Access-Control-Request-Method"] != null &&
			       context.Request.Headers["Access-Control-Request-Headers"] != null &&
			       context.Request.Headers["Origin"] != null;
		}
	}
}
