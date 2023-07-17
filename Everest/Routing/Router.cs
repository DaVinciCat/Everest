using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Everest.Collections;
using Everest.Http;
using Microsoft.Extensions.Logging;

namespace Everest.Routing
{
	public class Router : IRouter
	{
		public ILogger<Router> Logger { get; }

		public string[] Delimiters { get; set; } = { "/" };

		public RouteDescriptor[] Routes => methods.SelectMany(kvp => kvp.Value.Routes).ToArray();

		public Dictionary<string, Func<string, IRouteSegmentParser>> Parsers { get; set; } = new()
		{
			{ AlphaNumericRouteSegmentParser.SegmentPattern, value => new AlphaNumericRouteSegmentParser(value) },
			{ StringParameterRouteSegmentParser.SegmentPattern, value => new StringParameterRouteSegmentParser(value)},
			{ IntParameterRouteSegmentParser.SegmentPattern, value => new IntParameterRouteSegmentParser(value)},
			{ DoubleParameterRouteSegmentParser.SegmentPattern, value => new DoubleParameterRouteSegmentParser(value)},
			{ FloatParameterRouteSegmentParser.SegmentPattern, value => new FloatParameterRouteSegmentParser(value)},
			{ BoolParameterRouteSegmentParser.SegmentPattern, value => new BoolParameterRouteSegmentParser(value)},
			{ GuidParameterRouteSegmentParser.SegmentPattern, value => new GuidParameterRouteSegmentParser(value)},
			{ DateTimeParameterRouteSegmentParser.SegmentPattern, value => new DateTimeParameterRouteSegmentParser(value)}
		};

		private readonly Dictionary<string, RouteTrie> methods = new();
		
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
				routes = new RouteTrie(GetParser)
				{
					Delimiters = Delimiters
				};

				methods[descriptor.Route.HttpMethod] = routes;
			}
			
			routes.Insert(descriptor);

			IRouteSegmentParser GetParser(string segment)
			{
				foreach (var segmentPattern in Parsers.Keys)
				{
					var match = Regex.Match(segment, segmentPattern);
					if (match.Success)
					{
						return Parsers[segmentPattern](segment);
					}
				}

				throw new InvalidOperationException($"Unknown segment pattern: {segment}.");
			}
		}

		public async Task<bool> TryRouteAsync(HttpContext context)
		{
			if (context == null) 
				throw new ArgumentNullException(nameof(context));

			Logger.LogTrace($"{context.Id} - Try to route request: {context.Request.Description}");
			
			if (methods.TryGetValue(context.Request.HttpMethod, out var routes))
			{
				var result = (routes.SearchExact(context.Request.Path, out var descriptor, out var parameters) || routes.SearchBreadthFirst(context.Request.Path, out descriptor, out parameters));
				if (result)
				{
					context.Request.PathParameters = parameters;
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

		#region Trie

		private class TrieNode
		{
			public IRouteSegmentParser SegmentParser { get; }

			public string SegmentPattern { get; }

			public RouteDescriptor RouteDescriptor { get; }

			public int Level { get; }

			public bool IsTerminal => RouteDescriptor != null;
			
			public Dictionary<string, TrieNode> Children { get; } = new();

			public TrieNode()
			{

			}

			public TrieNode(IRouteSegmentParser segmentParser, string segmentPattern, int level, RouteDescriptor routeDescriptor = null)
			{
				SegmentParser = segmentParser ?? throw new ArgumentNullException(nameof(segmentParser));
				SegmentPattern = segmentPattern ?? throw new ArgumentNullException(nameof(segmentPattern));
				Level = level;
				RouteDescriptor = routeDescriptor;
			}
		}

		private class RouteTrie
		{
			public RouteDescriptor[] Routes => routes.Values.ToArray();

			public string[] Delimiters { get; set; } = { "/" };

			private readonly Dictionary<string, RouteDescriptor> routes = new();
			
			private readonly TrieNode root = new();

			private readonly Func<string, IRouteSegmentParser> getParser;

			public RouteTrie(Func<string, IRouteSegmentParser> getParser)
			{
				this.getParser = getParser;
			}

			public void Insert(RouteDescriptor descriptor)
			{
				if (descriptor == null)
					throw new ArgumentNullException(nameof(descriptor));

				if (routes.ContainsKey(descriptor.Route.Description))
				{
					throw new InvalidOperationException($"Duplicate route: {descriptor.Route.Description}");
				}

				var currentNode = root;
				var segments = descriptor.Route.RoutePattern.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries);

				for (var i = 0; i < segments.Length; i++)
				{
					var segment = segments[i];
					var isLastSegment = (i == segments.Length - 1);
					
					if (!currentNode.Children.ContainsKey(segment))
					{
						if (isLastSegment)
						{
							currentNode.Children[segment] = new TrieNode(getParser(segment), segment, currentNode.Level + 1, descriptor);
							routes.Add(descriptor.Route.Description, descriptor);
							return;
						}

						currentNode.Children[segment] = new TrieNode(getParser(segment), segment, currentNode.Level + 1);
					}

					currentNode = currentNode.Children[segment];
				}
			}

			public bool SearchBreadthFirst(string route, out RouteDescriptor descriptor, out ParameterCollection parameters)
			{
				if (route == null)
					throw new ArgumentNullException(nameof(route));

				descriptor = null;
				parameters = new ParameterCollection();

				var currentNode = root;
				var segments = route.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries);

				foreach (var segment in segments)
				{
					var any = false;

					foreach (var childNode in currentNode.Children.Values)
					{
						if (childNode.SegmentParser.TryParse(segment, out parameters))
						{
							currentNode = childNode;
							any = true;
							break;
						}
					}

					if (!any)
					{
						return false;
					}
				}

				descriptor = currentNode.RouteDescriptor;
				return currentNode.IsTerminal;
			}

			public bool SearchExact(string route, out RouteDescriptor descriptor, out ParameterCollection parameters)
			{
				parameters = new ParameterCollection();
				return routes.TryGetValue(route, out descriptor);
			}
		}

		#endregion
	}
}
