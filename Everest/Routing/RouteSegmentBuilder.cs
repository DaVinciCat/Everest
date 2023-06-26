using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Everest.Collections;

namespace Everest.Routing
{
	public delegate RouteSegment BuildRouteSegment(string value, RouteSegment next);

	public class RouteSegmentBuilderOptions
	{
		public char[] Delimiters { get; set;  } = { '/' };
	}

	public class RouteSegmentBuilder : IRouteSegmentBuilder
	{
		public Dictionary<string, BuildRouteSegment> Builders { get; set; } = new()
		{
			{ AlphaNumericRouteSegment.Pattern, (value, next) => new AlphaNumericRouteSegment(value, next) },
			{ IntParameterRouteSegment.Pattern, (value, next) => new IntParameterRouteSegment(value, next) },
			{ FloatParameterRouteSegment.Pattern, (value, next) => new FloatParameterRouteSegment(value, next) },
			{ DoubleParameterRouteSegment.Pattern, (value, next) => new FloatParameterRouteSegment(value, next) },
			{ GuidParameterRouteSegment.Pattern, (value, next) => new GuidParameterRouteSegment(value, next) },
			{ DateTimeParameterRouteSegment.Pattern, (value, next) => new DateTimeParameterRouteSegment(value, next) },
			{ BoolParameterRouteSegment.Pattern, (value, next) => new BoolParameterRouteSegment(value, next) },
			{ StringParameterRouteSegment.Pattern, (value, next) => new StringParameterRouteSegment(value, next) },
		};

		private readonly RouteSegmentBuilderOptions options;

		public RouteSegmentBuilder()
			: this(new RouteSegmentBuilderOptions())
		{
			
		}

		public RouteSegmentBuilder(RouteSegmentBuilderOptions options)
		{
			this.options = options ?? throw new ArgumentNullException(nameof(options));
		}

		public RouteSegment Build(string routePattern)
		{
			if (routePattern == null) 
				throw new ArgumentNullException(nameof(routePattern));
			
			var splitted = routePattern.Split(options.Delimiters, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			var segments = new Iterator<string>(splitted);

			return BuildImpl();

			RouteSegment BuildImpl()
			{
				if (!segments.MoveNext())
					throw new InvalidOperationException($"Invalid route pattern: {routePattern}.");

				var segment = segments.Current;
				if (segment == null)
					throw new ArgumentNullException(nameof(segment));

				foreach (var pattern in Builders.Keys)
				{
					var match = Regex.Match(segment, pattern);
					if (match.Success)
					{
						return Builders[pattern](match.Groups[0].Value, segments.HasNext() ? BuildImpl() : null);
					}
				}

				throw new InvalidOperationException($"Unsupported route segment: {segment}.");
			}
		}
	}
}
