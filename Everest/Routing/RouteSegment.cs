using System.Collections.Generic;
using System.Collections.Specialized;
using Everest.Utils;

namespace Everest.Routing
{
	public abstract class RouteSegment
	{
		public string Name { get; }

		public RouteSegment NextSegment { get; }

		public bool HasNextSegment => NextSegment != null;

		protected RouteSegment(string name, RouteSegment next)
		{
			Name = name;
			NextSegment = next;
		}

		public abstract bool TryParse(Iterator<string> iterator, NameValueCollection parameters);
	}

	public class StringRouteSegment : RouteSegment
	{
		public StringRouteSegment(string name, RouteSegment next) 
			: base(name, next)
		{

		}
		
		public override bool TryParse(Iterator<string> iterator, NameValueCollection parameters) 
		{
			if (!iterator.MoveNext())
			{
				return false;
			}

			var result = Name == iterator.Current;

			if (result && HasNextSegment)
			{
				return NextSegment.TryParse(iterator, parameters);
			}

			return result;
		}
	}

	public class ParamRouteSegment : RouteSegment
	{
		public ParamRouteSegment(string name, RouteSegment next) 
			: base(name, next)
		{
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

	public class SplatRouteSegment : RouteSegment
	{
		public static char Separator { get; set; } = '/';

		public SplatRouteSegment(string name, RouteSegment next)
			: base(name, next)
		{

		}

		public override bool TryParse(Iterator<string> iterator, NameValueCollection parameters)
		{
			var values = new List<string>();
			var result = true;

			if (!HasNextSegment)
			{
				while (iterator.MoveNext())
				{
					values.Add(iterator.Current);
				}
			}
			
			while (iterator.HasNext())
			{
				result = NextSegment.TryParse(iterator, parameters);
				if (result)
					break;

				values.Add(iterator.Current);
			}

			parameters.Add(Name, string.Join(Separator, values));
			return result;
		}
	}
}
