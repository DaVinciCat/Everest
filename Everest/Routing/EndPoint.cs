using System;
using System.Reflection;
using Everest.Http;

namespace Everest.Routing
{
	public class EndPoint
	{
		public string Description => $"{Type}.{MethodInfo.Name}()";

		public Type Type { get; }

		public MethodInfo MethodInfo { get; }

		public Action<HttpContext> Action { get; }
		
		public EndPoint(Type type, MethodInfo methodInfo, Action<HttpContext> action)
		{
			Type = type;
			MethodInfo = methodInfo;
			Action = action;
		}

		public void Invoke(HttpContext context)
		{
			Action.Invoke(context);
		}
	}
}
