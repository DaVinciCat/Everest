using Everest.Http;
using Microsoft.Extensions.Logging;

namespace Everest.Routing
{
	public interface IEndPointInvoker
	{
		void Invoke(HttpContext context, EndPoint endPoint);
	}

	public class EndPointInvoker : IEndPointInvoker
	{
		public ILogger<EndPointInvoker> Logger { get; }

		public EndPointInvoker(ILogger<EndPointInvoker> logger)
		{
			Logger = logger;
		}

		public void Invoke(HttpContext context, EndPoint endPoint)
		{
			Logger.LogTrace($"{context.Id} - Invoke route from: {context.Request.Description} to: {endPoint.Description}");
			endPoint.Invoke(context);
			Logger.LogTrace($"{context.Id} - Invoke complete");
		}
	}
}
