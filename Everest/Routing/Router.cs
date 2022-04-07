using Everest.Http;
using System;
using Microsoft.Extensions.Logging;

namespace Everest.Routing
{
	public class Router
	{
		public RouteCollection Routes { get; set; } = new RouteCollection();
		
		private readonly ILogger<Router> logger;

		public Router(ILogger<Router> logger)
		{
			this.logger = logger;
		}
		
		public void Route(HttpContext context)
		{
			var route = new Route(context.Request);

			try
			{
				logger.LogTrace($"{context.Request.Id}: Routing to: {route}");
				if (!Routes.TryGetRoute(route, out var action))
				{
					logger.LogWarning($"{context.Request.Id}: Route not found: {route}");
					context.Response.SendNotFound($"Requested route not found: {route}.");
					return;
				}

				logger.LogTrace($"{context.Request.Id}: Invoke route action for: {route}");
				action.Invoke(context);
				logger.LogTrace($"{context.Request.Id}: Invoke route action done for: {route}");
			}
			catch (Exception ex)
			{
				try
				{
					logger.LogError(ex, $"{context.Request.Id}: Invoke route action failed for: {route}");
					ErrorHandler?.Invoke(context, ex);
				}
				catch (Exception e)
				{
					logger.LogError(e, $"{context.Request.Id}: Error handling failed for: {route}");
				}
			}
			finally
			{
				context.Response.Close();
			}
		}

		public Action<HttpContext, Exception> ErrorHandler { get; set; } = (context, ex) =>
		{
			context.Response.SendInternalServerError($"Failed to process request: {context.Request.HttpMethod} {context.Request.EndPoint}.\r\n{ex.Message}");
		};
	}
}
