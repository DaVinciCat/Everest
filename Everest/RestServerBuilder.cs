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
			var builder = new RouteSegmentBuilder();
			var parser = new RouteSegmentParser();
			var routes = new RouteTable(builder, parser);
			var router = new Router(routes, loggerFactory.CreateLogger<Router>());
			var scanner = new RouteScanner(loggerFactory.CreateLogger<RouteScanner>());
			var logger = loggerFactory.CreateLogger<RestServer>();

			return new RestServer(router, scanner, logger);
		}
	}
}
