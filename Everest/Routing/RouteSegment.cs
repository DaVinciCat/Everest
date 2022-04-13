using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

	public class StringRouteSegment : RouteSegment
	{
		public StringRouteSegment(string value, RouteSegment next) 
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

	public class ParamRouteSegment : RouteSegment
	{
		public string Name { get; }
		
		public ParamRouteSegment(string value, string name, RouteSegment next) 
			: base(value, next)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentException("Parameter name is required");

			Name = name; 
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

	public class IntParamRouteSegment : ParamRouteSegment
	{
		private readonly Func<string, bool> isInt;

		public IntParamRouteSegment(string value, string name, Func<string, bool> isInt, RouteSegment next) 
			: base(value, name, next)
		{
			this.isInt = isInt;
		}

		public override bool TryParse(Iterator<string> iterator, NameValueCollection parameters)
		{
			if (!iterator.MoveNext())
			{
				return false;
			}

			if (!isInt(iterator.Current))
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
			Traverse(segment);
			return $"/{string.Join("/", segments)}";

			void Traverse(RouteSegment next)
			{
				segments.Add(next.Value);
				if (next.HasNextSegment)
					Traverse(next.NextSegment);
			}
		}
	}
}
