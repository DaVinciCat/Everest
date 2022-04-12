using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Everest.Utils;

namespace Everest.Routing
{
	public abstract class RouteSegment
	{
		public string Path { get; }

		public string FullPath
		{
			get
			{
				var segments = new List<string>();
				Traverse(this);
				return $"/{string.Join("/", segments)}";

				void Traverse(RouteSegment segment)
				{
					segments.Add(segment.Path);
					if (segment.HasNextSegment)
						Traverse(segment.NextSegment);
				}
			}
		}
			
		public RouteSegment NextSegment { get; }

		public bool HasNextSegment => NextSegment != null;

		protected RouteSegment(string path, RouteSegment next)
		{
			Path = path;
			NextSegment = next;
		}

		public abstract bool TryParse(Iterator<string> iterator, NameValueCollection parameters);
	}

	public class StringRouteSegment : RouteSegment
	{
		public StringRouteSegment(string path, RouteSegment next) 
			: base(path, next)
		{

		}
		
		public override bool TryParse(Iterator<string> iterator, NameValueCollection parameters) 
		{
			if (!iterator.MoveNext())
			{
				return false;
			}

			var result = Path == iterator.Current;

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
		
		public ParamRouteSegment(string path, string name, RouteSegment next) 
			: base(path, next)
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
}
