using System;
using Everest.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Everest.Logging
{
	public static class LoggingExtensions
	{
		private static class LoggerHolder<T>
		{
			public static ILogger<T> Logger { get; set; }
		}

		public static ILogger<T> GetLogger<T>(this HttpContext context)
		{
			if (context == null) 
				throw new ArgumentNullException(nameof(context));

			if (LoggerHolder<T>.Logger != null)
				return LoggerHolder<T>.Logger;

			var factory = context.Services.GetRequiredService<ILoggerFactory>();
			return LoggerHolder<T>.Logger = factory.CreateLogger<T>();
		}
	}
}
