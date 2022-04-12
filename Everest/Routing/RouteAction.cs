using Everest.Http;
using System;

namespace Everest.Routing
{
	public class RouteAction
	{
		public string HttpMethod { get; }

		public RouteSegment Segment { get; }

		public Action<HttpContext> Action { get; }

		public string Description => $"{HttpMethod} {Segment.Pattern}";

		public RouteAction(string httpMethod, RouteSegment segment, Action<HttpContext> action)
		{
			HttpMethod = httpMethod;
			Segment = segment;
			Action = action;
		}
	}
}
