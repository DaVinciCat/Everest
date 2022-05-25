using Everest.Http;
using System;
using Microsoft.Extensions.Logging;

namespace Everest.Routing
{
	public class Router
	{
		public RouteTable Routes { get; }

		private readonly ILogger<Router> logger;

		public Router(RouteTable routes, ILogger<Router> logger)
		{
			Routes = routes;
			this.logger = logger;
		}
		
		public void Route(HttpContext context)
		{
			try
			{
				logger.LogTrace($"{context.Request.Id} - Routing request for: {context.Request.Description}");
				if (!Routes.ResolveRoute(context, out var route))
				{
					logger.LogWarning($"{context.Request.Id} - Route not found");
					context.Response.SendNotFound($"Requested route not found: {context.Request.Description}.");
					return;
				}

				logger.LogTrace($"{context.Request.Id} - Routing from: {context.Request.Description}	to: {route.Description}");
				route.Invoke(context);
				logger.LogTrace($"{context.Request.Id} - Routing complete");
			}
			catch (Exception ex)
			{
				try
				{
					logger.LogError(ex, $"{context.Request.Id} - Routing failed");
					ErrorHandler?.Invoke(context, ex);
				}
				catch (Exception e)
				{
					logger.LogError(e, $"{context.Request.Id} - Routing error handling failed");
				}
			}
			finally
			{
				context.Response.Close();
			}
		}

		public Action<HttpContext, Exception> ErrorHandler { get; set; } = (context, ex) =>
		{
			context.Response.SendInternalServerError($"Failed to process request: {context.Request.Description}.\r\n{ex.Message}");
		};
	}
}
