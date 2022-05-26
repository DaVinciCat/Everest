using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using Everest.Utils;

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
		public static string Pattern => @"^[a-zA-Z0-9_]*$";

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

			var result = Value == iterator.Current;

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
		
		public ParameterRouteSegment(string value, RouteSegment next) 
			: base(value, next)
		{
			if (string.IsNullOrEmpty(value))
				throw new ArgumentException("Parameter name required.");

			var match = Regex.Match(value, "[^{}]+?(?=}|:)");
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

		public IntParameterRouteSegment(string value, RouteSegment next)
			: base(value, next)
		{
			var match = Regex.Match(value, "[^{}]+?(?=:int)");
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

			if (iterator.Current != null && !Regex.IsMatch(iterator.Current, "^-?[0-9]*$"))
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

		public FloatParameterRouteSegment(string value, RouteSegment next)
			: base(value, next)
		{
			var match = Regex.Match(value, "[^{}]+?(?=:float)");
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

			if (iterator.Current != null && !Regex.IsMatch(iterator.Current, "[+-]?([0-9]*[.])?[0-9]+"))
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

		public GuidParameterRouteSegment(string value, RouteSegment next)
			: base(value, next)
		{
			var match = Regex.Match(value, "[^{}]+?(?=:guid)");
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

			if (iterator.Current != null && !Regex.IsMatch(iterator.Current, "^([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$"))
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

		public DateTimeParameterRouteSegment(string value, RouteSegment next)
			: base(value, next)
		{
			var match = Regex.Match(value, "[^{}]+?(?=:datetime)");
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

			if (iterator.Current != null && !Regex.IsMatch(iterator.Current, @"\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d(?:\.\d+)?Z?"))
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

		public BoolParameterRouteSegment(string value, RouteSegment next)
			: base(value, next)
		{
			var match = Regex.Match(value, "[^{}]+?(?=:bool)");
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

			if (iterator.Current != null && !Regex.IsMatch(iterator.Current, "^(?i:true|false)$"))
				return false;

			parameters.Add(Name, iterator.Current);

			if (HasNextSegment)
			{
				return NextSegment.TryParse(iterator, parameters);
			}

			return true;
		}
	}
}
