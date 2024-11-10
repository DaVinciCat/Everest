using System;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Utils;
using Microsoft.Extensions.Logging;

namespace Everest.EndPoints
{
    public class EndPointInvoker : IEndPointInvoker, IHasLogger
	{
        ILogger IHasLogger.Logger => Logger;

        public ILogger<EndPointInvoker> Logger { get; }

		public EndPointInvoker(ILogger<EndPointInvoker> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<bool> TryInvokeEndPointAsync(IHttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			if (!context.TryGetEndPoint(out var endPoint))
			{
                Logger.LogTraceIfEnabled(() => $"{context.TraceIdentifier} - Failed to invoke endpoint. No route descriptor");
				return false;
			}

            Logger.LogTraceIfEnabled(() => $"{context.TraceIdentifier} - Try to invoke endpoint: {new { Request = context.Request.Description, Endpoint = endPoint.Description }}");
            await endPoint.InvokeAsync(context);

            Logger.LogTraceIfEnabled(() => $"{context.TraceIdentifier} - Successfully invoked endpoint: {new { Endpoint = endPoint.Description }}");
			return true;
		}
	}
}
