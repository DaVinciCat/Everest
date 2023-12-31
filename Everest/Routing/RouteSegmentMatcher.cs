﻿using System;
using System.Collections.Specialized;
using Everest.Collections;

namespace Everest.Routing
{
	public interface IRouteSegmentMatcher
	{
		bool TryMatch(RouteSegment segment, string endPoint, out NameValueCollection parameters);
	}

	public class RouteSegmentMatcher : IRouteSegmentMatcher
	{
		public char[] Delimiters { get; } = { '/' };

		public bool TryMatch(RouteSegment segment, string endPoint, out NameValueCollection parameters)
		{
			parameters = new NameValueCollection();

			var segments = endPoint.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			var iterator = new Iterator<string>(segments);

			return segment.TryParse(iterator, parameters) && !iterator.HasNext();
		}
	}
}

