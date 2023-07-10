using System;
using System.Collections.Generic;
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

		public RouteDescriptor[] Routes => methods.SelectMany(kvp => kvp.Value.Values).ToArray();

		private readonly Dictionary<string, Dictionary<string, RouteDescriptor>> methods = new();
		
		public Router(ILogger<Router> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public void RegisterRoute(RouteDescriptor descriptor)
		{
			if (descriptor == null)
				throw new ArgumentNullException(nameof(descriptor));

			if (!methods.TryGetValue(descriptor.Route.HttpMethod, out var routes))
			{
				routes = new Dictionary<string, RouteDescriptor>();
				methods[descriptor.Route.HttpMethod] = routes;
			}

			if (routes.Any(o => o.Value.Route.Description == descriptor.Route.Description))
				throw new InvalidOperationException($"Duplicate route: {descriptor.Route.Description}.");

			routes.Add(descriptor.Route.RoutePattern, descriptor);
		}

		public async Task<bool> TryRouteAsync(HttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			Logger.LogTrace($"{context.Id} - Try to route request: {context.Request.Description}");

			var httpMethod = context.Request.HttpMethod;
			var path = context.Request.Path;
			
			if (methods.TryGetValue(httpMethod, out var descriptors))
			{
				if (descriptors.TryGetValue(path, out var descriptor))
				{
					context.Features.Set<IRouteDescriptorFeature>(new RouteDescriptorFeature(descriptor));
					Logger.LogTrace($"{context.Id} - Successfully routed from: {context.Request.Description} to: {descriptor.Route.Description}");
					return true;
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
