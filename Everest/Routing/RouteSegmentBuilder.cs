using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Everest.Utils;

namespace Everest.Routing
{
	public delegate RouteSegment BuildRouteSegment(string name, RouteSegment next);
	
	public class RouteSegmentBuilder
	{
		public  Dictionary<string, BuildRouteSegment> Builders { get; } = new()
		{
			{@"^[a-z\d]+$", (name, next) => new StringRouteSegment(name, next)},
			{@"(?<=:)(\w+)", (name, next) => new ParamRouteSegment(name, next)},
			{@"[a-z\d]+(?=\*)", (name, next) => new SplatParamRouteSegment(name, next)},
		};

		public RouteSegment Build(string pattern)
		{
			if (string.IsNullOrEmpty(pattern))
				throw new ArgumentNullException(nameof(pattern), "Route pattern is reqired.");

			var split = pattern.SplitUrl();
			var iterator = new Iterator<string>(split);
	
			return BuildImpl();

			RouteSegment BuildImpl()
			{
				if (!iterator.MoveNext())
					throw new ArgumentException($"Invalid route pattern: {pattern}.", nameof(pattern));

				var current = iterator.Current;
				if (current == null)
					throw new ArgumentException("Segment required.");

				foreach (var regex in Builders.Keys)
				{
					var match = Regex.Match(current, regex);
					if (match.Success)
					{
						return Builders[regex](match.Groups[0].Value, iterator.HasNext() ? BuildImpl() : null);
					}
				}

				throw new ArgumentException($"Unsupported route segment: '{current}'.");
			}
		}
	}
}
