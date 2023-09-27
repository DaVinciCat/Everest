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
		
		public RouteSegmentParserCollection SegmentParsers { get; } = new RouteSegmentParserCollection
        {
			{ AlphaNumericRouteSegmentParser.SegmentPattern, segment => new AlphaNumericRouteSegmentParser(segment) },
			{ StringParameterRouteSegmentParser.SegmentPattern, segment => new StringParameterRouteSegmentParser(segment)},
			{ IntParameterRouteSegmentParser.SegmentPattern, segment => new IntParameterRouteSegmentParser(segment)},
			{ DoubleParameterRouteSegmentParser.SegmentPattern, segment => new DoubleParameterRouteSegmentParser(segment)},
			{ FloatParameterRouteSegmentParser.SegmentPattern, segment => new FloatParameterRouteSegmentParser(segment)},
			{ BoolParameterRouteSegmentParser.SegmentPattern, segment => new BoolParameterRouteSegmentParser(segment)},
			{ GuidParameterRouteSegmentParser.SegmentPattern, segment => new GuidParameterRouteSegmentParser(segment)},
			{ DateTimeParameterRouteSegmentParser.SegmentPattern, segment => new DateTimeParameterRouteSegmentParser(segment)}
		};

		private readonly Dictionary<string, RouteTrie> methods = new Dictionary<string, RouteTrie>();

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
				foreach (var segmentPattern in SegmentParsers)
				{
					var match = Regex.Match(segment, segmentPattern);
					if (match.Success)
					{
						return SegmentParsers[segmentPattern](segment);
					}
				}

				throw new InvalidOperationException($"Unknown segment pattern: {segment}.");
			}
		}

		public async Task<bool> TryRouteAsync(HttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			Logger.LogTrace($"{context.TraceIdentifier} - Try to match requested route: {new { Request = context.Request.Description }}");

			if (methods.TryGetValue(context.Request.HttpMethod, out var routes))
			{
				var result = routes.SearchExact(context.Request.Path, out var descriptor, out var parameters) || routes.SearchBreadthFirst(context.Request.Path, out descriptor, out parameters);
				if (result)
				{
					context.Request.PathParameters = parameters;
					context.Features.Set<IRouteDescriptorFeature>(new RouteDescriptorFeature(descriptor));
					Logger.LogTrace($"{context.TraceIdentifier} - Successfully matched requested route: {new { Request = context.Request.Description, RoutePattern = descriptor.Route.Description, EndPoint = descriptor.EndPoint.Description }}");
					return true;
				}
			}

			await OnRouteNotFoundAsync(context);
			Logger.LogWarning($"{context.TraceIdentifier} - Failed to match requested route. Requested route not found: {new { Request = context.Request.Description }}");
			return false;
		}

		public Func<HttpContext, Task> OnRouteNotFoundAsync { get; set; } = async context =>
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			context.Response.KeepAlive = false;
			context.Response.StatusCode = HttpStatusCode.NotFound;
			context.Response.Clear();
			await context.Response.WriteTextAsync($"Requested route not found: {context.Request.Description}");
		};

		#region Trie

		private class TrieNode
		{
			public IRouteSegmentParser SegmentParser { get; }

			public string SegmentPattern { get; }

			public RouteDescriptor RouteDescriptor { get; internal set; }

			public int Level { get; }

			public bool IsTerminal => RouteDescriptor != null;

			public Dictionary<string, TrieNode> Children { get; } = new Dictionary<string, TrieNode>();

			public TrieNode()
			{

			}

			public TrieNode(IRouteSegmentParser segmentParser, string segmentPattern, int level)
			{
				SegmentParser = segmentParser ?? throw new ArgumentNullException(nameof(segmentParser));
				SegmentPattern = segmentPattern ?? throw new ArgumentNullException(nameof(segmentPattern));
				Level = level;
			}
		}

		private class RouteTrie
		{
			public RouteDescriptor[] Routes => routes.Values.ToArray();

			public string[] Delimiters { get; set; } = { "/" };

			private readonly Dictionary<string, RouteDescriptor> routes = new Dictionary<string, RouteDescriptor>();

			private readonly TrieNode root = new TrieNode();

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
				if (segments.Length == 0)
				{
					routes.Add("", descriptor);
					return;
				}

				for (var i = 0; i < segments.Length; i++)
				{
					var segment = segments[i];
					var isLastSegment = (i == segments.Length - 1);

					if (!currentNode.Children.ContainsKey(segment))
					{
						currentNode.Children[segment] = new TrieNode(getParser(segment), segment, currentNode.Level + 1);
					}

					if (isLastSegment)
					{
						routes.Add(descriptor.Route.Description, descriptor);
						currentNode.Children[segment].RouteDescriptor = descriptor;
						return;
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
				var segments = route.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries).ToIterator();
				while (segments.MoveNext())
				{
					var any = false;

					foreach (var childNode in currentNode.Children.Values)
					{
						if (childNode.SegmentParser.TryParse(segments, out var segmentParameters))
						{
							parameters.Add(segmentParameters);
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
