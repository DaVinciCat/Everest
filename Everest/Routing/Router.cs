using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Everest.Collections;
using Everest.Http;
using Microsoft.Extensions.Logging;

namespace Everest.Routing
{
	public class Router : IRouter
	{
		public ILogger<Router> Logger { get; }

		public RouteDescriptor[] Routes => methods.SelectMany(kvp => kvp.Value).ToArray();

		private readonly Dictionary<string, HashSet<RouteDescriptor>> methods = new();

		private readonly Dictionary<string, RouteDescriptor> staticRoutes = new();

		private readonly IRouteSegmentParser parser;
		
		public Router(IRouteSegmentParser parser, ILogger<Router> logger)
		{
			this.parser = parser ?? throw new ArgumentNullException(nameof(parser));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public void RegisterRoute(RouteDescriptor descriptor)
		{
			if (descriptor == null)
				throw new ArgumentNullException(nameof(descriptor));

			if (descriptor.Segment.IsCompletelyStatic())
			{
				var path = descriptor.Segment.GetFullRoutePath();
				staticRoutes.Add(path, descriptor);
				return;
			}

			if (!methods.TryGetValue(descriptor.Route.HttpMethod, out var routes))
			{
				routes = new HashSet<RouteDescriptor>();
				methods[descriptor.Route.HttpMethod] = routes;
			}
			
			if (routes.Any(o => o.Route.Description == descriptor.Route.Description))
				throw new InvalidOperationException($"Duplicate route: {descriptor.Route.Description}.");

			routes.Add(descriptor);
		}

		public async Task<bool> TryRouteAsync(HttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			Logger.LogTrace($"{context.Id} - Try route request: {context.Request.Description}");

			var httpMethod = context.Request.HttpMethod;
			var path = context.Request.Path;
			
			if (staticRoutes.TryGetValue(path, out var staticDescriptor))
			{
				context.Features.Set<IRouteDescriptorFeature>(new RouteDescriptorFeature(staticDescriptor));
				Logger.LogTrace($"{context.Id} - Successfully routed from: {context.Request.Description} to: {staticDescriptor.Route.Description}");
				return true;
			}

			if (methods.TryGetValue(httpMethod, out var descriptors))
			{
				foreach (var descriptor in descriptors)
				{
					var parameters = new NameValueCollection();
					if (await parser.TryParseAsync(descriptor.Segment, path, parameters))
					{
						context.Request.PathParameters = new ParameterCollection(parameters);
						context.Features.Set<IRouteDescriptorFeature>(new RouteDescriptorFeature(descriptor));
						Logger.LogTrace($"{context.Id} - Successfully routed from: {context.Request.Description} to: {descriptor.Route.Description}");
						return true;
					}
				}
			}

			await OnRouteNotFoundAsync(context);
			Logger.LogWarning($"{context.Id} - Failed to route request. Requested route not found: {context.Request.Description}");
			return false;
		}

		public Func<HttpContext, Task> OnRouteNotFoundAsync { get; set; } = async context =>
		{
			if (context == null) 
				throw new ArgumentNullException(nameof(context));

			context.Response.KeepAlive = false;
			context.Response.StatusCode = HttpStatusCode.NotFound;
			await context.Response.WriteJsonAsync($"Requested route not found: {context.Request.Description}");
		};
	}
}
