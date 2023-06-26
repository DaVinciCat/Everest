using System;
using System.Reflection;
using Everest.Http;

namespace Everest.EndPoints
{
	public class EndPoint
	{
		public string Description => $"{Type}.{MethodInfo.Name}()";

		public Type Type { get; }

		public MethodInfo MethodInfo { get; }

		public Action<HttpContext> Action { get; }
		
		public EndPoint(Type type, MethodInfo methodInfo, Action<HttpContext> action)
		{
			Type = type ?? throw new ArgumentNullException(nameof(type));
			MethodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
			Action = action ?? throw new ArgumentNullException(nameof(action));
		}

		public void Invoke(HttpContext context)
		{
			if(context == null)
				throw new ArgumentNullException(nameof(context));

			Action.Invoke(context);
		}
	}
}
