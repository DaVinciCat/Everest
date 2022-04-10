using System.Collections.Generic;
using System.Collections.Specialized;
using Everest.Utils;

namespace Everest.Routing
{
	public abstract class RouteSegment
	{
		public string Name { get; }

		public RouteSegment Next { get; }

		public bool HasNext => Next != null;

		protected RouteSegment(string name, RouteSegment next)
		{
			Name = name;
			Next = next;
		}

		public abstract bool Match(Iterator<string> iterator, RouteSegment segment, NameValueCollection parameters);
	}

	public class StringRouteSegment : RouteSegment
	{
		public StringRouteSegment(string name, RouteSegment next) 
			: base(name, next)
		{

		}
		
		public override bool Match(Iterator<string> iterator, RouteSegment segment, NameValueCollection parameters) 
		{
			if (!iterator.MoveNext())
			{
				return false;
			}

			var current = iterator.Current;
			var result = segment.Name == current;

			if (result && segment.HasNext)
			{
				return segment.Next.Match(iterator, segment.Next, parameters);
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

		public override bool Match(Iterator<string> iterator, RouteSegment segment, NameValueCollection parameters)
		{
			if (!iterator.MoveNext())
			{
				return false;
			}

			parameters.Add(segment.Name, iterator.Current);

			if (segment.HasNext)
			{
				return segment.Next.Match(iterator, segment.Next, parameters);
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

		public override bool Match(Iterator<string> iterator, RouteSegment segment, NameValueCollection parameters)
		{
			if (!segment.HasNext)
			{
				while (iterator.MoveNext()) { }
				return true;
			}
			
			var values = new List<string>();
			var result = true;
			while (iterator.HasNext())
			{
				result = segment.Next.Match(iterator, segment.Next, parameters);
				if (result)
					break;

				values.Add(iterator.Current);
			}

			parameters.Add(segment.Name, string.Join(Separator, values));
			return result;
		}
	}
}
