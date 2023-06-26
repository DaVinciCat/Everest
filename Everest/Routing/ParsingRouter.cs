using System;
using System.Collections.Generic;
using System.Linq;
using Everest.Collections;
using Everest.Http;
using Microsoft.Extensions.Logging;

namespace Everest.Routing
{
	public class ParsingRouter : IRouter
	{
		public ILogger<ParsingRouter> Logger { get; }

		public RouteDescriptor[] RoutingTable => methods.SelectMany(kvp => kvp.Value).ToArray();

		private readonly Dictionary<string, HashSet<RouteDescriptor>> methods = new();

		private readonly IRouteSegmentParser parser;
		
		public ParsingRouter(IRouteSegmentParser parser, ILogger<ParsingRouter> logger)
		{
			this.parser = parser ?? throw new ArgumentNullException(nameof(parser));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public void RegisterRoute(RouteDescriptor descriptor)
		{
			if (descriptor == null)
				throw new ArgumentNullException(nameof(descriptor));

			if (!methods.TryGetValue(descriptor.Route.HttpMethod, out var routes))
			{
				routes = new HashSet<RouteDescriptor>();
				methods[descriptor.Route.HttpMethod] = routes;
			}

			if (routes.Any(o => o.Route.Description == descriptor.Route.Description))
				throw new InvalidOperationException($"Duplicate route: {descriptor.Route.Description}.");

			routes.Add(descriptor);
		}

		public bool TryRoute(HttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			Logger.LogTrace($"{context.Id} - Try route request: {context.Request.Description}");

			var httpMethod = context.Request.HttpMethod;
			var endPoint = context.Request.EndPoint;

			if (!methods.TryGetValue(httpMethod, out var descriptors))
			{
				OnRouteNotFound(context);
				Logger.LogWarning($"{context.Id} - Failed to route request. Unsupported HTTP method: {httpMethod}");
				return false;
			}

			foreach (var descriptor in descriptors)
			{
				if (parser.TryParse(descriptor.Segment, endPoint, out var parameters))
				{
					context.Request.PathParameters = new ParameterCollection(parameters);
					context.Features.Set<IRouteDescriptorFeature>(new RouteDescriptorFeature(descriptor));
					Logger.LogTrace($"{context.Id} - Successfully routed from: {context.Request.Description} to: {descriptor.Route.Description}");
					return true;
				}
			}

			OnRouteNotFound(context);
			Logger.LogWarning($"{context.Id} - Failed to route request. Requested route not found: {context.Request.Description}");
			return false;
		}

		public Action<HttpContext> OnRouteNotFound { get; set; } = context =>
		{
			context.Response.Write404NotFound($"Requested route not found: {context.Request.Description}");
		};
	}
}
