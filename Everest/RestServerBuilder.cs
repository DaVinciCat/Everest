using Everest.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Everest
{
	public static class RestServerBuilder
	{
		public static RestServer Build()
		{
			var factory = new NullLoggerFactory();
			var builder = new RouteSegmentBuilder();
			var parser = new RouteSegmentParser();
			var scanner = new RouteScanner(factory.CreateLogger<RouteScanner>());

			var router = new Router(builder, parser, factory.CreateLogger<Router>());
			var services = new ServiceCollection();

			return new RestServer(router, scanner, services, factory.CreateLogger<RestServer>());
		}

		public static RestServer Build(ILoggerFactory factory)
		{
			var builder = new RouteSegmentBuilder();
			var parser = new RouteSegmentParser();
			var scanner = new RouteScanner(factory.CreateLogger<RouteScanner>());

			var router = new Router(builder, parser, factory.CreateLogger<Router>());
			var services = new ServiceCollection();

			return new RestServer(router, scanner, services, factory.CreateLogger<RestServer>());
		}

		public static RestServer Build(IServiceCollection services)
		{
			var factory = new NullLoggerFactory();
			var builder = new RouteSegmentBuilder();
			var parser = new RouteSegmentParser();
			var scanner = new RouteScanner(factory.CreateLogger<RouteScanner>());

			var router = new Router(builder, parser, factory.CreateLogger<Router>());

			return new RestServer(router, scanner, services, factory.CreateLogger<RestServer>());
		}

		public static RestServer Build(IServiceCollection services, ILoggerFactory factory)
		{
			var builder = new RouteSegmentBuilder();
			var parser = new RouteSegmentParser();
			var scanner = new RouteScanner(factory.CreateLogger<RouteScanner>());

			var router = new Router(builder, parser, factory.CreateLogger<Router>());

			return new RestServer(router, scanner, services, factory.CreateLogger<RestServer>());
		}
	}
}
