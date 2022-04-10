namespace Everest.Routing
{
	public abstract class RouteSegment
	{
		public string Type { get;}

		public string Name { get; }

		public RouteSegment Next { get; }

		public bool HasNext => Next != null;

		protected RouteSegment(string type, string name, RouteSegment next)
		{
			Type = type;
			Name = name;
			Next = next;
		}
	}

	public class StringRouteSegment : RouteSegment
	{
		public new static string Type => "STRING";

		public StringRouteSegment(string name, RouteSegment next) 
			: base(Type, name, next)
		{

		}
	}

	public class ParamRouteSegment : RouteSegment
	{
		public new static string Type => "PARAM";

		public ParamRouteSegment(string name, RouteSegment next) 
			: base(Type, name, next)
		{
		}
	}

	public class SplatRouteSegment : RouteSegment
	{
		public new static string Type => "SPLAT";

		public SplatRouteSegment(string name, RouteSegment next)
			: base(Type, name, next)
		{

		}
	}
}
