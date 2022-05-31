using Everest.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Everest.Annotations;

namespace Everest.Routing
{
	public class RouteScanner
	{
		public ILogger<RouteScanner> Logger { get; }

		public RouteScanner(ILogger<RouteScanner> logger)
		{
			Logger = logger;
		}

		public IEnumerable<Route> Scan(Assembly assembly)
		{
			var count = 0;

			Logger.LogTrace($"Scanning assembly {assembly} for routes");
			foreach (var type in GetRestResourceTypes(assembly))
			{
				var routePrefix = GetAttributes<RestResourceAttribute>(type).FirstOrDefault()?.RoutePrefix;

				Logger.LogTrace($"Scanning type {type} for routes");
				foreach (var method in GetRestRouteMethods(type))
				{
					var attribute = GetAttributes<RestRouteAttribute>(method).FirstOrDefault();
					if (attribute != null)
					{
						var action = (Action<HttpContext>)method.CreateDelegate(typeof(Action<HttpContext>), null);
						var route = new Route(attribute.HttpMethod, $"{routePrefix}/{attribute.RoutePattern}", action);
						Logger.LogTrace($"Route found	- {route.Description} at {type}.{method.Name}()");
						count++;

						yield return route;
					}
				}
			}

			Logger.LogTrace($"Scan of assembly {assembly} complete");
			Logger.LogTrace($"Total routes found: {count}");
		}

		private static IEnumerable<Type> GetRestResourceTypes(Assembly assembly)
		{
			foreach (var type in assembly
				.GetTypes()
				.Where(t => t.IsClass && t.IsDefined(typeof(RestResourceAttribute), false)))
				yield return type;
		}

		private static IEnumerable<MethodInfo> GetRestRouteMethods(Type type)
		{
			foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
				.Where(m => !m.IsAbstract && m.IsDefined(typeof(RestRouteAttribute), true)))
				yield return method;
		}

		public static IEnumerable<T> GetAttributes<T>(MethodInfo method) 
			where T : Attribute
		{
			foreach (var attribute in method.GetCustomAttributes(typeof(T), false).OfType<T>()) 
				yield return attribute;
		}

		public static IEnumerable<T> GetAttributes<T>(Type type)
			where T : Attribute
		{
			foreach (var attribute in type.GetCustomAttributes(typeof(T), false).OfType<T>())
				yield return attribute;
		}
	}
}
