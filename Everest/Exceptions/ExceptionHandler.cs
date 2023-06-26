using System;
using Everest.Http;
using Microsoft.Extensions.Logging;

namespace Everest.Exceptions
{
	public class ExceptionHandler : IExceptionHandler
	{
		public ILogger<ExceptionHandler> Logger { get; }
		
		public ExceptionHandler(ILogger<ExceptionHandler> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public void Handle(HttpContext context, Exception ex)
		{
			if (context == null) 
				throw new ArgumentNullException(nameof(context));
			
			if (ex == null) 
				throw new ArgumentNullException(nameof(ex));

			Logger.LogError(ex, $"{context.Id} - {ex.Message}");
			OnException(context, ex);
		}

		public Action<HttpContext, Exception> OnException { get; set; } = (context, ex) =>
		{
			context.Response.Write500InternalServerError($"Failed to process request: {context.Request.Description}.{Environment.NewLine}{ex.Message}");
		};
	}
}