using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using Everest.Collections;

namespace Everest.Routing
{
	public abstract class RouteSegment
	{
		public string Value { get; }

		public RouteSegment NextSegment { get; }

		public bool HasNextSegment => NextSegment != null;

		protected RouteSegment(string value, RouteSegment next)
		{
			Value = value;
			NextSegment = next;
		}

		public abstract bool TryParse(Iterator<string> iterator, NameValueCollection parameters);
	}

	public class AlphaNumericRouteSegment : RouteSegment
	{
		public static string Pattern => @"^[a-zA-Z0-9-_\.]+$";

		public AlphaNumericRouteSegment(string value, RouteSegment next)
			: base(value, next)
		{

		}

		public override bool TryParse(Iterator<string> iterator, NameValueCollection parameters)
		{
			if (!iterator.MoveNext())
			{
				return false;
			}

			var result = string.CompareOrdinal(Value, iterator.Current) == 0;

			if (result && HasNextSegment)
			{
				return NextSegment.TryParse(iterator, parameters);
			}

			return result;
		}
	}

	public class ParameterRouteSegment : RouteSegment
	{
		public static string Pattern => @"({\w+})";

		public string Name { get; }

		private static readonly Regex MatchRegex = new("[^{}]+?(?=}|:)", RegexOptions.Compiled);

		public ParameterRouteSegment(string value, RouteSegment next)
			: base(value, next)
		{
			if (string.IsNullOrEmpty(value))
				throw new ArgumentException("Parameter name required.");

			var match = MatchRegex.Match(value);
			if (!match.Success)
				throw new ArgumentException($"Invalid parameter pattern {value}.");

			Name = match.Groups[0].Value;
		}

		public override bool TryParse(Iterator<string> iterator, NameValueCollection parameters)
		{
			if (!iterator.MoveNext())
			{
				return false;
			}

			parameters.Add(Name, iterator.Current);

			if (HasNextSegment)
			{
				return NextSegment.TryParse(iterator, parameters);
			}

			return true;
		}
	}

	public class IntParameterRouteSegment : RouteSegment
	{
		public static string Pattern => @"({\w+:int})";

		public string Name { get; }

		private static readonly Regex MatchRegex = new("[^{}]+?(?=:int)", RegexOptions.Compiled);

		private static readonly Regex ParseRegex = new("^-?[0-9]*$", RegexOptions.Compiled);

		public IntParameterRouteSegment(string value, RouteSegment next)
			: base(value, next)
		{
			var match = MatchRegex.Match(value);
			if (!match.Success)
				throw new ArgumentException($"Invalid int parameter pattern {value}.");

			Name = match.Groups[0].Value;
		}

		public override bool TryParse(Iterator<string> iterator, NameValueCollection parameters)
		{
			if (!iterator.MoveNext())
			{
				return false;
			}

			if (iterator.Current != null && !ParseRegex.IsMatch(iterator.Current))
				return false;

			parameters.Add(Name, iterator.Current);

			if (HasNextSegment)
			{
				return NextSegment.TryParse(iterator, parameters);
			}

			return true;
		}
	}

	public class FloatParameterRouteSegment : RouteSegment
	{
		public static string Pattern => @"({\w+:float})";

		public string Name { get; }

		private static readonly Regex MatchRegex = new("[^{}]+?(?=:float)", RegexOptions.Compiled);

		private static readonly Regex ParseRegex = new("[+-]?([0-9]*[.])?[0-9]+", RegexOptions.Compiled);

		public FloatParameterRouteSegment(string value, RouteSegment next)
			: base(value, next)
		{
			var match = MatchRegex.Match(value);
			if (!match.Success)
				throw new ArgumentException($"Invalid float parameter pattern {value}.");

			Name = match.Groups[0].Value;
		}

		public override bool TryParse(Iterator<string> iterator, NameValueCollection parameters)
		{
			if (!iterator.MoveNext())
			{
				return false;
			}

			if (iterator.Current != null && !ParseRegex.IsMatch(iterator.Current))
				return false;

			parameters.Add(Name, iterator.Current);

			if (HasNextSegment)
			{
				return NextSegment.TryParse(iterator, parameters);
			}

			return true;
		}
	}

	public class GuidParameterRouteSegment : RouteSegment
	{
		public static string Pattern => @"({\w+:guid})";

		public string Name { get; }

		private static readonly Regex MatchRegex = new("[^{}]+?(?=:guid)", RegexOptions.Compiled);

		private static readonly Regex ParseRegex = new("^([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$", RegexOptions.Compiled);

		public GuidParameterRouteSegment(string value, RouteSegment next)
			: base(value, next)
		{
			var match = MatchRegex.Match(value);
			if (!match.Success)
				throw new ArgumentException($"Invalid guid parameter pattern {value}.");

			Name = match.Groups[0].Value;
		}

		public override bool TryParse(Iterator<string> iterator, NameValueCollection parameters)
		{
			if (!iterator.MoveNext())
			{
				return false;
			}

			if (iterator.Current != null && !ParseRegex.IsMatch(iterator.Current))
				return false;

			parameters.Add(Name, iterator.Current);

			if (HasNextSegment)
			{
				return NextSegment.TryParse(iterator, parameters);
			}

			return true;
		}
	}

	public class DateTimeParameterRouteSegment : RouteSegment
	{
		public static string Pattern => @"({\w+:datetime})";

		public string Name { get; }

		private static readonly Regex MatchRegex = new("[^{}]+?(?=:datetime)", RegexOptions.Compiled);

		private static readonly Regex ParseRegex = new(@"\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d(?:\.\d+)?Z?", RegexOptions.Compiled);

		public DateTimeParameterRouteSegment(string value, RouteSegment next)
			: base(value, next)
		{
			var match = MatchRegex.Match(value);
			if (!match.Success)
				throw new ArgumentException($"Invalid datetime parameter pattern {value}.");

			Name = match.Groups[0].Value;
		}

		public override bool TryParse(Iterator<string> iterator, NameValueCollection parameters)
		{
			if (!iterator.MoveNext())
			{
				return false;
			}

			if (iterator.Current != null && !ParseRegex.IsMatch(iterator.Current))
				return false;

			parameters.Add(Name, iterator.Current);

			if (HasNextSegment)
			{
				return NextSegment.TryParse(iterator, parameters);
			}

			return true;
		}
	}

	public class BoolParameterRouteSegment : RouteSegment
	{
		public static string Pattern => @"({\w+:bool})";

		public string Name { get; }

		private static readonly Regex MatchRegex = new("[^{}]+?(?=:bool)", RegexOptions.Compiled);

		private static readonly Regex ParseRegex = new(@"^(?i:true|false)$", RegexOptions.Compiled);

		public BoolParameterRouteSegment(string value, RouteSegment next)
			: base(value, next)
		{
			var match = MatchRegex.Match(value);
			if (!match.Success)
				throw new ArgumentException($"Invalid bool parameter pattern {value}.");

			Name = match.Groups[0].Value;
		}

		public override bool TryParse(Iterator<string> iterator, NameValueCollection parameters)
		{
			if (!iterator.MoveNext())
			{
				return false;
			}

			if (iterator.Current != null && !ParseRegex.IsMatch(iterator.Current))
				return false;

			parameters.Add(Name, iterator.Current);

			if (HasNextSegment)
			{
				return NextSegment.TryParse(iterator, parameters);
			}

			return true;
		}
	}

	public static class RouteSegmentExtensions
	{
		public static string GetPath(this RouteSegment segment)
		{
			var segments = new List<string>();
			GetPathImpl(segment);
			return $"/{string.Join("/", segments)}";

			void GetPathImpl(RouteSegment next)
			{
				segments.Add(next.Value);
				if (next.HasNextSegment)
					GetPathImpl(next.NextSegment);
			}
		}
	}
}
