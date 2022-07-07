using System;
using Everest.Http;
using Microsoft.Extensions.Logging;

namespace Everest.Middleware
{
	public class ExceptionHandlingMiddleware : MiddlewareBase
	{
		private readonly ILogger<ExceptionHandlingMiddleware> logger;

		public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
		{
			this.logger = logger;
		}

		public override void Invoke(HttpContext context)
		{
			try
			{
				if(HasNext)
					Next.Invoke(context);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, ex.Message);
				context.Response.Write500InternalServerError($"Failed to process request: {context.Request.Description}.\r\n{ex.Message}");
			}
		}
	}
}
