using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Everest.Utils;

namespace Everest.Routing
{
	public delegate bool RouteSegmentMatchAction(Iterator<string> iterator, RouteSegment segment, NameValueCollection parameters);

	public class RouteSegmentMatcher
	{
		public static char Separator { get; set; } = '/';

		public Dictionary<string, RouteSegmentMatchAction> Matchers { get; }
		
		public RouteSegmentMatcher()
		{
			Matchers = new Dictionary<string, RouteSegmentMatchAction>
			{
				{ StringRouteSegment.Type, MatchStringRouteSegment },
				{ ParamRouteSegment.Type, MatchParamRouteSegment },
				{ SplatRouteSegment.Type, MatchSplatRouteSegment }
			};
		}

		private bool MatchStringRouteSegment(Iterator<string> iterator, RouteSegment segment, NameValueCollection parameters)
		{
			if (!iterator.MoveNext())
			{
				return false;
			}

			var current = iterator.Current;
			var result = segment.Name == current;

			if (result && segment.HasNext)
			{
				if (!Matchers.TryGetValue(segment.Next.Type, out var action))
					throw new ArgumentException($"Route segment type is not supported: {segment.Next.Type}.");

				return action(iterator, segment.Next, parameters);
			}

			return result;
		}

		private bool MatchParamRouteSegment(Iterator<string> iterator, RouteSegment segment, NameValueCollection parameters)
		{
			if (!iterator.MoveNext())
			{
				return false;
			}

			parameters.Add(segment.Name, iterator.Current);

			if (segment.HasNext)
			{
				if (!Matchers.TryGetValue(segment.Next.Type, out var action))
					throw new ArgumentException($"Route segment type is not supported: {segment.Next.Type}.");

				return action(iterator, segment.Next, parameters);
			}

			return true;
		}

		private bool MatchSplatRouteSegment(Iterator<string> iterator, RouteSegment segment, NameValueCollection parameters)
		{
			if (!segment.HasNext)
			{
				while (iterator.MoveNext()) { }
				return true;
			}
			
			if (!Matchers.TryGetValue(segment.Next.Type, out var action))
				throw new ArgumentException($"Route segment type is not supported: {segment.Next.Type}.");

			var values = new List<string>();
			var result = true;
			while (iterator.HasNext())
			{
				result = action(iterator, segment.Next, parameters);
				if(result)
					break;

				values.Add(iterator.Current);
			}

			parameters.Add(segment.Name, string.Join(Separator, values));
			return result;
		}

		public bool Match(RouteSegment segment, string url, NameValueCollection parameters = null)
		{
			parameters ??= new NameValueCollection();

			var split = url.SplitUrl();
			var iterator = new Iterator<string>(split);

			if (!Matchers.TryGetValue(segment.Type, out var action))
				throw new ArgumentException($"Route segment type is not supported: {segment.Type}.");
			
			return action(iterator, segment, parameters) && !iterator.HasNext();
		}

	}
}

