using Everest.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Everest
{
	public static class RestServerBuilder
	{
		public static RestServer Build()
		{
			return Build(new NullLoggerFactory());
		}

		public static RestServer Build(ILoggerFactory loggerFactory)
		{
			var router = new Router(loggerFactory.CreateLogger<Router>());
			var logger = loggerFactory.CreateLogger<RestServer>();

			return new RestServer(router, logger);
		}
	}
}
