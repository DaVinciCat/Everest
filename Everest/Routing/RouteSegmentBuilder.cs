using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Everest.Utils;

namespace Everest.Routing
{
	public delegate RouteSegment BuildRouteSegment(string value, RouteSegment next);
	
	public class RouteSegmentBuilder
	{
		public  Dictionary<string, BuildRouteSegment> Builders { get; } = new()
		{
			{@"^[a-z\d]+$", (value, next) => new StringRouteSegment(value, next)},
			{@"({\w+})", (value, next) => new ParameterRouteSegment(value, ExtractParameterName(value), next)},
			{@"({\w+:int})", (value, next) => new IntParameterRouteSegment(value, ExtractIntParameterName(value), IsInt, next )},
			{@"({\w+:guid})", (value, next) => new GuidParameterRouteSegment(value, ExtractGuidParameterName(value), IsGuid, next) }
		};

		public RouteSegment Build(string pattern)
		{
			if (string.IsNullOrEmpty(pattern))
				throw new ArgumentNullException(nameof(pattern), "Route pattern is required.");

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

				throw new ArgumentException($"Unsupported route segment: {segment}.");
			}
		}

		private static string ExtractParameterName(string value)
		{
			var match = Regex.Match(value, "[^{}].+?(?=}|:)");
			if (!match.Success)
				throw new ArgumentException($"Invalid parameter pattern {value}.");
			
			return match.Groups[0].Value;
		}

		private static string ExtractIntParameterName(string value)
		{
			var match = Regex.Match(value, "[^{}].+?(?=:int)");
			if (!match.Success)
				throw new ArgumentException($"Invalid int parameter pattern {value}.");

			return match.Groups[0].Value;
		}

		private static string ExtractGuidParameterName(string value)
		{
			var match = Regex.Match(value, "[^{}].+?(?=:guid)");
			if (!match.Success)
				throw new ArgumentException($"Invalid guid parameter pattern {value}");

			return match.Groups[0].Value;
		}

		private static bool IsInt(string value)
		{
			return Regex.IsMatch(value, "^-?[0-9]*$");
		}

		private static bool IsGuid(string value)
		{
			return Regex.IsMatch(value, "^([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$");
		}
	}
}
