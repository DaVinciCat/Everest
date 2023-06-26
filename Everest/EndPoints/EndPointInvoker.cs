using System;
using Everest.Http;
using Everest.Routing;
using Microsoft.Extensions.Logging;

namespace Everest.EndPoints
{
	public class EndPointInvoker : IEndPointInvoker
	{
		public ILogger<EndPointInvoker> Logger { get; }
		
		public EndPointInvoker(ILogger<EndPointInvoker> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public bool TryInvokeEndPoint(HttpContext context)
		{
			if (context == null) 
				throw new ArgumentNullException(nameof(context));
			
			if (!context.TryGetRouteDescriptor(out var descriptor))
			{
				Logger.LogError($"{context.Id} - Failed to invoke endpoint. No route descriptor");
				return false;
			}

			Logger.LogTrace($"{context.Id} - Try invoke endpoint from: {context.Request.Description} to: {descriptor.EndPoint.Description}");
			descriptor.EndPoint.Invoke(context);
			Logger.LogTrace($"{context.Id} - Successfully invoked endpoint: {descriptor.EndPoint.Description}");

			return true;
		}
	}
}
