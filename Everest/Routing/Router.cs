using Everest.Http;
using System;
using Microsoft.Extensions.Logging;

namespace Everest.Routing
{
	public class Router
	{
		public RouteCollection Routes { get; }

		public RouteSegmentBuilder RouteBuilder { get; } = new();

		public RouteSegmentParser RouteParser { get; } = new();

		private readonly ILogger<Router> logger;

		public Router(ILogger<Router> logger)
		{
			this.Routes = new RouteCollection(RouteBuilder, RouteParser);
			this.logger = logger;
		}
		
		public void Route(HttpContext context)
		{
			try
			{
				logger.LogTrace($"{context.Request.Id}: Find route for: '{context.Request.Description}'");
				if (!Routes.TryGetRouteAction(context, out var routeAction))
				{
					logger.LogWarning($"{context.Request.Id}: Route not found for: '{context.Request.Description}'");
					context.Response.SendNotFound($"Requested route not found: '{context.Request.Description}'.");
					return;
				}

				logger.LogTrace($"{context.Request.Id}: Route from: '{context.Request.Description}' to: '{routeAction.Description}' ");
				logger.LogTrace($"{context.Request.Id}: Invoke route action for: '{context.Request.Description}'");
				routeAction.Action.Invoke(context);
				logger.LogTrace($"{context.Request.Id}: Invoke route action done for: '{context.Request.Description}'");
			}
			catch (Exception ex)
			{
				try
				{
					logger.LogError(ex, $"{context.Request.Id}: Invoke route action failed for: '{context.Request.Description}'");
					ErrorHandler?.Invoke(context, ex);
				}
				catch (Exception e)
				{
					logger.LogError(e, $"{context.Request.Id}: Error handling failed for: '{context.Request.Description}'");
				}
			}
			finally
			{
				context.Response.Close();
			}
		}

		public Action<HttpContext, Exception> ErrorHandler { get; set; } = (context, ex) =>
		{
			context.Response.SendInternalServerError($"Failed to process request: '{context.Request.Description}'.\r\n{ex.Message}");
		};
	}
}
