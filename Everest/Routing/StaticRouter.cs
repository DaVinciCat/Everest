using System;
using System.Collections.Generic;
using System.Linq;
using Everest.Collections;
using Everest.Http;
using Microsoft.Extensions.Logging;

namespace Everest.Routing
{
	public class StaticRouter : IRouter
	{
		public ILogger<StaticRouter> Logger { get; }

		public RouteDescriptor[] RoutingTable => methods.SelectMany(kvp => kvp.Value.Values).ToArray();

		private readonly Dictionary<string, Dictionary<string, RouteDescriptor>> methods = new();

		public StaticRouter(ILogger<StaticRouter> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public void RegisterRoute(RouteDescriptor descriptor)
		{
			if (descriptor == null)
				throw new ArgumentNullException(nameof(descriptor));

			if (!descriptor.Segment.IsCompletelyStatic())
				throw new InvalidOperationException("Route is not completely static.");

			if (!methods.TryGetValue(descriptor.Route.HttpMethod, out var routes))
			{
				routes = new Dictionary<string, RouteDescriptor>();
				methods[descriptor.Route.HttpMethod] = routes;
			}

			if (routes.Any(o => o.Value.Route.Description == descriptor.Route.Description))
				throw new InvalidOperationException($"Duplicate route: {descriptor.Route.Description}.");

			var path = descriptor.Segment.GetFullRoutePath();
			routes.Add(path, descriptor);
		}

		public bool TryRoute(HttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			Logger.LogTrace($"{context.Id} - Route request: {context.Request.Description}");

			var httpMethod = context.Request.HttpMethod;
			var endPoint = context.Request.EndPoint;
			
			if (!methods.TryGetValue(httpMethod, out var descriptors))
			{
				OnRouteNotFound(context);
				Logger.LogWarning($"{context.Id} - Unsupported HTTP method: {httpMethod}");
				return false;
			}

			if (!descriptors.TryGetValue(endPoint, out var descriptor))
			{
				OnRouteNotFound(context);
				Logger.LogWarning($"{context.Id} - Requested route not found: {context.Request.Description}");
				return false;
			}

			context.Features.Set<IRouteDescriptorFeature>(new RouteDescriptorFeature(descriptor));
			Logger.LogTrace($"{context.Id} - Routed from: {context.Request.Description} to: {descriptor.Route.Description}");
			return true;
		}

		public Action<HttpContext> OnRouteNotFound { get; set; } = context =>
		{
			context.Response.Write404NotFound($"Requested route not found: {context.Request.Description}");
		};
	}
}
