﻿using System;
using System.Net;
using System.Threading.Tasks;
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

		public async Task HandleExceptionAsync(IHttpContext context, Exception ex)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			if (ex == null)
				throw new ArgumentNullException(nameof(ex));

            if (Logger.IsEnabled(LogLevel.Error))
                Logger.LogError(ex, $"{context.TraceIdentifier} - {new { ExceptionMessage = ex.Message }}");

			await OnExceptionAsync(context, ex);
		}

		public Func<IHttpContext, Exception, Task> OnExceptionAsync { get; set; } = async (context, ex) =>
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			if (ex == null)
				throw new ArgumentNullException(nameof(ex));

			if(ex is HttpListenerException)
				return;

			if (!context.Response.ResponseSent)
			{
				context.Response.KeepAlive = false;
				context.Response.StatusCode = HttpStatusCode.InternalServerError;
				await context.Response.SendTextResponseAsync($"Failed to process request: {context.Request.Description}{Environment.NewLine}{ex}");
			}
		};
	}
}