using System;
using System.Reflection;
using Everest.Http;

namespace Everest
{
	public static class RestServerExtensions
	{
		public static void RegisterRoute(this RestServer server, string httpMethod, string pattern, Action<HttpContext> action)
		{
			server.Router.Routes.RegisterRoute(httpMethod, pattern, action);
		}

		public static void ScanRoutes(this RestServer server, Assembly assembly)
		{
			foreach (var route in server.RouteScanner.Scan(assembly))
			{
				server.Router.Routes.RegisterRoute(route);	
			}
		}
	}
}
