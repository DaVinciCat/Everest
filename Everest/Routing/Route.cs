using Everest.Http;
using System;
using System.Reflection;

namespace Everest.Routing
{
	public class Route
	{
		public string Description => $"{HttpMethod} {Pattern}";

		public string HttpMethod { get; }

		public string Pattern { get; }
		
		public Action<HttpContext> Action { get; }

		public MethodInfo MethodInfo { get; }

		public Route(string httpMethod, string pattern, Action<HttpContext> action)
		{
			HttpMethod = httpMethod;
			Pattern = pattern;
			Action = action;
		}

		public Route(MethodInfo methodInfo, string httpMethod, string pattern, Action<HttpContext> action)
			: this(httpMethod, pattern, action)
		{
			MethodInfo = methodInfo;
		}

		public void Invoke(HttpContext context)
		{
			Action.Invoke(context);
		}
	}
}
