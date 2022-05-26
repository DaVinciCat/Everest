using Everest.Http;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using Everest.Log;
using Microsoft.Extensions.Logging;

namespace Everest.Routing
{
	public class Router
	{
		public ILogger<Router> Logger { get; set; } = DefaultLogger.CreateLogger<Router>();
		
		public RouteSegmentBuilder RouteBuilder { get; set; } = new();

		public RouteSegmentParser RouteParser { get; set; } = new();

		public RouteScanner RouteScanner { get; set; } = new();
		
		private readonly Dictionary<string, Dictionary<RouteSegment, Route>> methods = new();
		
		public Action<HttpContext, Exception> ErrorHandler { get; set; } = (context, ex) =>
		{
			context.Response.SendInternalServerError($"Failed to process request: {context.Request.Description}.\r\n{ex.Message}");
		};

		public void Route(HttpContext context)
		{
			try
			{
				Logger.LogTrace($"{context.Request.Id} - Routing request for: {context.Request.Description}");
				if (!ResolveRoute(context, out var route))
				{
					Logger.LogWarning($"{context.Request.Id} - Route not found");
					context.Response.SendNotFound($"Requested route not found: {context.Request.Description}.");
					return;
				}

				Logger.LogTrace($"{context.Request.Id} - Routing from: {context.Request.Description}	to: {route.Description}");
				route.Invoke(context);
				Logger.LogTrace($"{context.Request.Id} - Routing complete");
			}
			catch (Exception ex)
			{
				try
				{
					Logger.LogError(ex, $"{context.Request.Id} - Routing failed");
					ErrorHandler?.Invoke(context, ex);
				}
				catch (Exception e)
				{
					Logger.LogError(e, $"{context.Request.Id} - Routing error handling failed");
				}
			}
			finally
			{
				context.Response.Close();
			}
		}

		public void RegisterRoute(string httpMethod, string pattern, Action<HttpContext> action)
		{
			RegisterRoute(new Route(httpMethod, pattern, action));
		}

		public void RegisterRoute(Route route)
		{
			if (!methods.TryGetValue(route.HttpMethod, out var actions))
			{
				actions = new Dictionary<RouteSegment, Route>();
				methods.Add(route.HttpMethod, actions);
			}

			if (actions.Any(o => o.Value.Description == route.Description))
				throw new ArgumentException($"Route {route.Description} is already registered");

			var segment = RouteBuilder.Build(route.Pattern);
			actions.Add(segment, route);

			Logger.LogTrace($"Route registered - {route.Description}");
		}

		public void ScanRoutes(Assembly assembly)
		{
			foreach (var route in RouteScanner.Scan(assembly).ToArray())
			{
				RegisterRoute(route);
			}
		}

		private bool ResolveRoute(HttpContext context, out Route route)
		{
			route = null;

			var httpMethod = context.Request.HttpMethod;
			var url = context.Request.EndPoint;

			if (!methods.TryGetValue(httpMethod, out var routes))
				return false;

			foreach (var (key, value) in routes)
			{
				var parameters = new NameValueCollection();
				if (RouteParser.TryParse(key, url, parameters))
				{
					context.Request.PathParameters = new ParametersCollection(parameters);
					route = value;
					return true;
				}
			}

			return false;
		}
	}
}
