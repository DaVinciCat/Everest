using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Everest.Collections;

namespace Everest.Routing
{
	public interface IRouteSegmentBuilder
	{
		RouteSegment Build(string routePattern);
	}
	
	public delegate RouteSegment BuildRouteSegment(string value, RouteSegment next);
	
	public class RouteSegmentBuilder : IRouteSegmentBuilder
	{
		public char[] Delimiters { get; } = { '/' };

		public Dictionary<string, BuildRouteSegment> Builders { get; } = new()
		{
			{ AlphaNumericRouteSegment.Pattern, (value, next) => new AlphaNumericRouteSegment(value, next) },
			{ ParameterRouteSegment.Pattern, (value, next) => new ParameterRouteSegment(value, next) },
			{ IntParameterRouteSegment.Pattern, (value, next) => new IntParameterRouteSegment(value, next) },
			{ FloatParameterRouteSegment.Pattern, (value, next) => new FloatParameterRouteSegment(value, next) },
			{ GuidParameterRouteSegment.Pattern, (value, next) => new GuidParameterRouteSegment(value, next) },
			{ DateTimeParameterRouteSegment.Pattern, (value, next) => new DateTimeParameterRouteSegment(value, next) },
			{ BoolParameterRouteSegment.Pattern, (value, next) => new BoolParameterRouteSegment(value, next) }
		};

		public RouteSegment Build(string routePattern)
		{
			if (string.IsNullOrEmpty(routePattern))
				throw new ArgumentNullException(nameof(routePattern), "Route pattern required.");

			var segments = routePattern.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			var iterator = new Iterator<string>(segments);

			return BuildImpl();

			RouteSegment BuildImpl()
			{
				if (!iterator.MoveNext())
					throw new ArgumentException($"Invalid route pattern: {routePattern}.", nameof(routePattern));

				var segment = iterator.Current;
				if (segment == null)
					throw new ArgumentException("Segment required.");

				foreach (var pattern in Builders.Keys)
				{
					var match = Regex.Match(segment, pattern);
					if (match.Success)
					{
						return Builders[pattern](match.Groups[0].Value, iterator.HasNext() ? BuildImpl() : null);
					}
				}

				throw new ArgumentException($"Unsupported route segment: {segment}.");
			}
		}
	}
}
