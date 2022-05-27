using System;
using System.Reflection;
using Everest.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Everest
{
	public static class RestServerExtensions
	{
		public static void RegisterRoute(this RestServer server, string httpMethod, string routePattern, Action<HttpContext> action)
		{
			server.Router.RegisterRoute(httpMethod, routePattern, action);
		}

		public static void ScanRoutes(this RestServer server, Assembly assembly)
		{
			server.Router.ScanRoutes(assembly);
		}

		public static void RegisterTransientService<TService>(this RestServer server, Func<TService> factory)
			where TService : class
		{
			server.Services.AddTransient(_ => factory());
		}

		public static void RegisterScopedService<TService>(this RestServer server, Func<TService> factory)
			where TService : class
		{
			server.Services.AddScoped(_ => factory());
		}

		public static void RegisterSingletonService<TService>(this RestServer server, Func<TService> factory)
			where TService : class
		{
			server.Services.AddSingleton(_ => factory());
		}

		public static void RegisterTransientService<TService, TImplementation>(this RestServer server, Func<TImplementation> factory) 
			where TImplementation : class, TService
			where TService : class
		{
			server.Services.AddTransient<TService, TImplementation>(_ => factory());
		}

		public static void RegisterScopedService<TService, TImplementation>(this RestServer server, Func<TImplementation> factory)
			where TImplementation : class, TService
			where TService : class
		{
			server.Services.AddScoped(_ => factory());
		}

		public static void RegisterSingletonService<TService, TImplementation>(this RestServer server, Func<TImplementation> factory)
			where TImplementation : class, TService
			where TService : class
		{
			server.Services.AddSingleton(_ => factory());
		}
	}
}
