using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Everest.Utils;

namespace Everest.Routing
{
	public abstract class RouteSegment
	{
		public string Content { get; }

		public string Description
		{
			get
			{
				var segments = new List<string>();
				Traverse(this);
				return string.Join("/", segments);

				void Traverse(RouteSegment segment)
				{
					segments.Add(segment.Content);
					if (segment.HasNextSegment)
						Traverse(segment.NextSegment);
				}
			}
		}

		public bool IsOptional { get; }
			
		public RouteSegment NextSegment { get; }

		public bool HasNextSegment => NextSegment != null;

		protected RouteSegment(string content, RouteSegment next)
		{
			Content = content;
			NextSegment = next;
		}

		public abstract bool TryParse(Iterator<string> iterator, NameValueCollection parameters);
	}

	public class StringRouteSegment : RouteSegment
	{
		public StringRouteSegment(string content, RouteSegment next) 
			: base(content, next)
		{

		}
		
		public override bool TryParse(Iterator<string> iterator, NameValueCollection parameters) 
		{
			if (!iterator.MoveNext())
			{
				return false;
			}

			var result = Content == iterator.Current;

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
		
		public ParamRouteSegment(string content, string name, RouteSegment next) 
			: base(content, next)
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
