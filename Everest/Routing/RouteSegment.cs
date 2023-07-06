using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Everest.Collections;

namespace Everest.Routing
{
	public abstract class RouteSegment
	{
		public string Value { get; }

		public RouteSegment NextSegment { get; }

		public bool HasNextSegment => NextSegment != null;

		public abstract bool IsStatic { get; }

		protected RouteSegment(string value, RouteSegment next)
		{
			Value = value ?? throw new ArgumentNullException(nameof(value));
			NextSegment = next;
		}

		public abstract Task<bool> TryParseAsync(Iterator<string> segments, NameValueCollection parameters);
	}

	public class AlphaNumericRouteSegment : RouteSegment
	{
		public override bool IsStatic => true;

		public static string Pattern => @"^[a-zA-Z0-9-_\.]+$";

		public AlphaNumericRouteSegment(string value, RouteSegment next)
			: base(value, next)
		{

		}

		public override async Task<bool> TryParseAsync(Iterator<string> segments, NameValueCollection parameters)
		{
			if (segments == null) 
				throw new ArgumentNullException(nameof(segments));
			
			if (parameters == null) 
				throw new ArgumentNullException(nameof(parameters));
			
			if (!segments.MoveNext())
			{
				return false;
			}

			var result = string.CompareOrdinal(Value, segments.Current) == 0;

			if (result && HasNextSegment)
			{
				return await NextSegment.TryParseAsync(segments, parameters);
			}

			return result;
		}
	}

	public class StringParameterRouteSegment : RouteSegment
	{
		public override bool IsStatic => false;

		public static string Pattern => @"({\w+:string})";

		public string ParameterName { get; }

		public StringParameterRouteSegment(string value, RouteSegment next)
			: base(value, next)
		{
			if (string.IsNullOrEmpty(value))
				throw new ArgumentException("Parameter name required.");

			var match = Regex.Match(value, "[^{}]+?(?=}|:string)");
			if (!match.Success)
				throw new ArgumentException($"Invalid parameter pattern {value}.");

			ParameterName = match.Groups[0].Value;
		}

		public override async Task<bool> TryParseAsync(Iterator<string> segments, NameValueCollection parameters)
		{
			if (segments == null) 
				throw new ArgumentNullException(nameof(segments));
			
			if (parameters == null) 
				throw new ArgumentNullException(nameof(parameters));

			if (!segments.MoveNext())
			{
				return false;
			}

			parameters.Add(ParameterName, segments.Current);

			if (HasNextSegment)
			{
				return await NextSegment.TryParseAsync(segments, parameters);
			}

			return true;
		}
	}

	public class IntParameterRouteSegment : RouteSegment
	{
		public override bool IsStatic => false;

		public static string Pattern => @"({\w+:int})";

		public string Name { get; }

		public IntParameterRouteSegment(string value, RouteSegment next)
			: base(value, next)
		{
			var match = Regex.Match(value, "[^{}]+?(?=:int)");
			if (!match.Success)
				throw new ArgumentException($"Invalid int parameter pattern {value}.");

			Name = match.Groups[0].Value;
		}

		public override async Task<bool> TryParseAsync(Iterator<string> segments, NameValueCollection parameters)
		{
			if (segments == null)
				throw new ArgumentNullException(nameof(segments));

			if (parameters == null)
				throw new ArgumentNullException(nameof(parameters));

			if (!segments.MoveNext())
			{
				return false;
			}

			if (segments.Current != null && !Regex.IsMatch(segments.Current, @"^-?\d+$"))
				return false;

			parameters.Add(Name, segments.Current);

			if (HasNextSegment)
			{
				return await NextSegment.TryParseAsync(segments, parameters);
			}

			return true;
		}
	}

	public class FloatParameterRouteSegment : RouteSegment
	{
		public override bool IsStatic => false;

		public static string Pattern => @"({\w+:float})";

		public string Name { get; }

		public FloatParameterRouteSegment(string value, RouteSegment next)
			: base(value, next)
		{
			var match = Regex.Match(value, "[^{}]+?(?=:float)");
			if (!match.Success)
				throw new ArgumentException($"Invalid float parameter pattern {value}.");

			Name = match.Groups[0].Value;
		}

		public override async Task<bool> TryParseAsync(Iterator<string> segments, NameValueCollection parameters)
		{
			if (segments == null)
				throw new ArgumentNullException(nameof(segments));

			if (parameters == null)
				throw new ArgumentNullException(nameof(parameters));

			if (!segments.MoveNext())
			{
				return false;
			}

			if (segments.Current != null && !Regex.IsMatch(segments.Current, @"^-?\d+(\.\d+)?$"))
				return false;

			parameters.Add(Name, segments.Current);

			if (HasNextSegment)
			{
				return await NextSegment.TryParseAsync(segments, parameters);
			}

			return true;
		}
	}

	public class DoubleParameterRouteSegment : RouteSegment
	{
		public override bool IsStatic => false;

		public static string Pattern => @"({\w+:double})";

		public string Name { get; }

		public DoubleParameterRouteSegment(string value, RouteSegment next)
			: base(value, next)
		{
			var match = Regex.Match(value, "[^{}]+?(?=:double)");
			if (!match.Success)
				throw new ArgumentException($"Invalid float parameter pattern {value}.");

			Name = match.Groups[0].Value;
		}

		public override async Task<bool> TryParseAsync(Iterator<string> segments, NameValueCollection parameters)
		{
			if (segments == null)
				throw new ArgumentNullException(nameof(segments));

			if (parameters == null)
				throw new ArgumentNullException(nameof(parameters));

			if (!segments.MoveNext())
			{
				return false;
			}

			if (segments.Current != null && !Regex.IsMatch(segments.Current, @"^-?\d+(\.\d+)?$"))
				return false;

			parameters.Add(Name, segments.Current);

			if (HasNextSegment)
			{
				return await NextSegment.TryParseAsync(segments, parameters);
			}

			return true;
		}
	}

	public class GuidParameterRouteSegment : RouteSegment
	{
		public override bool IsStatic => false;

		public static string Pattern => @"({\w+:guid})";

		public string Name { get; }

		public GuidParameterRouteSegment(string value, RouteSegment next)
			: base(value, next)
		{
			var match = Regex.Match(value, "[^{}]+?(?=:guid)");
			if (!match.Success)
				throw new ArgumentException($"Invalid guid parameter pattern {value}.");

			Name = match.Groups[0].Value;
		}

		public override async Task<bool> TryParseAsync(Iterator<string> segments, NameValueCollection parameters)
		{
			if (segments == null)
				throw new ArgumentNullException(nameof(segments));

			if (parameters == null)
				throw new ArgumentNullException(nameof(parameters));

			if (!segments.MoveNext())
			{
				return false;
			}

			if (segments.Current != null && !Regex.IsMatch(segments.Current, "^([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$"))
				return false;

			parameters.Add(Name, segments.Current);

			if (HasNextSegment)
			{
				return await NextSegment.TryParseAsync(segments, parameters);
			}

			return true;
		}
	}

	public class DateTimeParameterRouteSegment : RouteSegment
	{
		public override bool IsStatic => false;

		public static string Pattern => @"({\w+:datetime})";

		public string Name { get; }

		public DateTimeParameterRouteSegment(string value, RouteSegment next)
			: base(value, next)
		{
			var match = Regex.Match(value, "[^{}]+?(?=:datetime)");
			if (!match.Success)
				throw new ArgumentException($"Invalid datetime parameter pattern {value}.");

			Name = match.Groups[0].Value;
		}

		public override async Task<bool> TryParseAsync(Iterator<string> segments, NameValueCollection parameters)
		{
			if (segments == null)
				throw new ArgumentNullException(nameof(segments));

			if (parameters == null)
				throw new ArgumentNullException(nameof(parameters));

			if (!segments.MoveNext())
			{
				return false;
			}

			if (segments.Current != null && !Regex.IsMatch(segments.Current, @"\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d(?:\.\d+)?Z?"))
				return false;

			parameters.Add(Name, segments.Current);

			if (HasNextSegment)
			{
				return await NextSegment.TryParseAsync(segments, parameters);
			}

			return true;
		}
	}

	public class BoolParameterRouteSegment : RouteSegment
	{
		public override bool IsStatic => false;

		public static string Pattern => @"({\w+:bool})";

		public string Name { get; }

		public BoolParameterRouteSegment(string value, RouteSegment next)
			: base(value, next)
		{
			var match = Regex.Match(value, "[^{}]+?(?=:bool)");
			if (!match.Success)
				throw new ArgumentException($"Invalid bool parameter pattern {value}.");

			Name = match.Groups[0].Value;
		}

		public override async Task<bool> TryParseAsync(Iterator<string> segments, NameValueCollection parameters)
		{
			if (segments == null)
				throw new ArgumentNullException(nameof(segments));

			if (parameters == null)
				throw new ArgumentNullException(nameof(parameters));

			if (!segments.MoveNext())
			{
				return false;
			}

			if (segments.Current != null && !Regex.IsMatch(segments.Current, "^(?i:true|false)$"))
				return false;

			parameters.Add(Name, segments.Current);

			if (HasNextSegment)
			{
				return await NextSegment.TryParseAsync(segments, parameters);
			}

			return true;
		}
	}

	public static class RouteSegmentExtensions
	{
		public static string GetFullRoutePath(this RouteSegment segment)
		{
			if (segment == null) 
				throw new ArgumentNullException(nameof(segment));

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

		public static bool IsCompletelyStatic(this RouteSegment segment)
		{
			if (segment == null) 
				throw new ArgumentNullException(nameof(segment));

			if (!segment.IsStatic)
				return false;

			if (segment.HasNextSegment)
				return IsCompletelyStatic(segment.NextSegment);

			return true;
		}
	}
}
