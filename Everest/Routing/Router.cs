using Everest.Http;
using System;
using Microsoft.Extensions.Logging;

namespace Everest.Routing
{
	public class Router
	{
		public RouteCollection Routes { get; }

		public RouteSegmentBuilder RouteBuilder { get; } = new();

		public RouteSegmentMatcher RouteMatcher { get; } = new();

		private readonly ILogger<Router> logger;

		public Router(ILogger<Router> logger)
		{
			this.Routes = new RouteCollection(RouteBuilder, RouteMatcher);
			this.logger = logger;
		}
		
		public void Route(HttpContext context)
		{
			var description = $"{context.Request.HttpMethod} {context.Request.EndPoint}";

			try
			{
				logger.LogTrace($"{context.Request.Id}: Find route for: {description}");
				if (!Routes.TryGetRoute(context, out var action))
				{
					logger.LogWarning($"{context.Request.Id}: Route not found for: {description}");
					context.Response.SendNotFound($"Requested route not found: {description}.");
					return;
				}

				logger.LogTrace($"{context.Request.Id}: Invoke route action for: {description}");
				action.Invoke(context);
				logger.LogTrace($"{context.Request.Id}: Invoke route action done for: {description}");
			}
			catch (Exception ex)
			{
				try
				{
					logger.LogError(ex, $"{context.Request.Id}: Invoke route action failed for: {description}");
					ErrorHandler?.Invoke(context, ex);
				}
				catch (Exception e)
				{
					logger.LogError(e, $"{context.Request.Id}: Error handling failed for: {description}");
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
