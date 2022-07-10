using Everest.Collections;
using Everest.Http;
using Microsoft.Extensions.Logging;

namespace Everest.Routing
{
	public interface IEndPointResolver
	{
		bool TryResolve(HttpContext context, out EndPoint endPoint);
	}

	public class EndPointResolver : IEndPointResolver
	{
		public ILogger<EndPointResolver> Logger { get; }

		private readonly IRouteCollection routes;

		public EndPointResolver(IRouteCollection routes, ILogger<EndPointResolver> logger)
		{
			this.routes = routes;
			Logger = logger;
		}

		public bool TryResolve(HttpContext context, out EndPoint endPoint)
		{
			endPoint = null;

			Logger.LogTrace($"{context.Id} - Routing request for: {context.Request.Description}");
			if (!routes.TryGetRoute(context, out var descriptor))
			{
				Logger.LogWarning($"{context.Id} - Route not found");
				return false;
			}

			endPoint = descriptor.EndPoint;
			Logger.LogTrace($"{context.Id} - Route found from: {descriptor.Route.Description} to {descriptor.EndPoint.Description}");
			return true;
		}
	}
}
