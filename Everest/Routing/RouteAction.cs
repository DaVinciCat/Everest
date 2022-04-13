using Everest.Http;
using System;

namespace Everest.Routing
{
	public class RouteAction
	{
		public string HttpMethod { get; }

		public RouteSegment Segment { get; }
		
		public string Description => $"{HttpMethod} {Segment.GetPath()}";

		private readonly Action<HttpContext> action;

		public RouteAction(string httpMethod, RouteSegment segment, Action<HttpContext> action)
		{
			HttpMethod = httpMethod;
			Segment = segment;

			this.action = action;
		}

		public void Invoke(HttpContext context)
		{
			action.Invoke(context);
		}
	}
}
