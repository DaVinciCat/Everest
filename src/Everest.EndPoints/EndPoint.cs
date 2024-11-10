using System;
using System.Reflection;
using System.Threading.Tasks;
using Everest.Http;

namespace Everest.EndPoints
{
	public class EndPoint
	{
		public string Description => $"{Type}.{MethodInfo.Name}()";

		public Type Type { get; }

		public MethodInfo MethodInfo { get; }

		public Func<IHttpContext, Task> Action { get; }
		
		public EndPoint(Type type, MethodInfo methodInfo, Func<IHttpContext, Task> action)
		{
			Type = type ?? throw new ArgumentNullException(nameof(type));
			MethodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
			Action = action ?? throw new ArgumentNullException(nameof(action));
		}

		public async Task InvokeAsync(IHttpContext context)
		{
			if(context == null)
				throw new ArgumentNullException(nameof(context));

			await Action.Invoke(context);
		}
	}
}
