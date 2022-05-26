using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Everest.Log
{
	public static class DefaultLogger
	{
		public static ILoggerFactory LoggerFactory { get; set; } = new NullLoggerFactory();

		public static ILogger<T> CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
	}
}
