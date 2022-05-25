using Everest.Http;
using System;

namespace Everest.Routing
{
	public class Route
	{
		public string Description => $"{HttpMethod} {Pattern}";

		public string HttpMethod { get; }

		public string Pattern { get; }
		
		public Action<HttpContext> Action { get; }

		public Route(string httpMethod, string pattern, Action<HttpContext> action)
		{
			HttpMethod = httpMethod;
			Pattern = pattern;
			Action = action;
		}

		public void Invoke(HttpContext context)
		{
			Action.Invoke(context);
		}
	}
}
