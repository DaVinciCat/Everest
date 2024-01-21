using System;
using System.Threading.Tasks;
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

		public async Task<bool> TryInvokeEndPointAsync(IHttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			if (!context.TryGetRouteDescriptor(out var descriptor))
			{
                if (Logger.IsEnabled(LogLevel.Trace))
                    Logger.LogTrace($"{context.TraceIdentifier} - Failed to invoke endpoint. No route descriptor");

				return false;
			}

            if (Logger.IsEnabled(LogLevel.Trace))
                Logger.LogTrace($"{context.TraceIdentifier} - Try to invoke endpoint: {new { Request = context.Request.Description, Endpoint = descriptor.EndPoint.Description }}");
			
            await descriptor.EndPoint.InvokeAsync(context);

            if (Logger.IsEnabled(LogLevel.Trace)) 
                Logger.LogTrace($"{context.TraceIdentifier} - Successfully invoked endpoint: {new { Endpoint = descriptor.EndPoint.Description }}");

			return true;
		}
	}
}
