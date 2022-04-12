using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Everest.Utils;

namespace Everest.Routing
{
	public delegate RouteSegment BuildRouteSegment(string path, RouteSegment next);
	
	public class RouteSegmentBuilder
	{
		public  Dictionary<string, BuildRouteSegment> Builders { get; } = new()
		{
			{@"^[a-z\d]+$", (path, next) => new StringRouteSegment(path, next)},
			{@"({\w+})", (path, next) => new ParamRouteSegment(path, Regex.Replace(path, "[{}]+", ""), next)}
		};

		public RouteSegment Build(string pattern)
		{
			if (string.IsNullOrEmpty(pattern))
				throw new ArgumentNullException(nameof(pattern), "Route pattern is reqired.");

			var segments = pattern.TrimStart('/').TrimEnd('/').Split("/");
			var iterator = new Iterator<string>(segments);
	
			return BuildImpl();

			RouteSegment BuildImpl()
			{
				if (!iterator.MoveNext())
					throw new ArgumentException($"Invalid route pattern: {pattern}.", nameof(pattern));

				var segment = iterator.Current;
				if (segment == null)
					throw new ArgumentException("Segment required.");

				foreach (var regex in Builders.Keys)
				{
					var match = Regex.Match(segment, regex);
					if (match.Success)
					{
						return Builders[regex](match.Groups[0].Value, iterator.HasNext() ? BuildImpl() : null);
					}
				}

				throw new ArgumentException($"Unsupported route segment: '{segment}'.");
			}
		}
	}
}
