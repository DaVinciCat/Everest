using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Everest.EndPoints;
using Everest.Http;
using Microsoft.Extensions.Logging;

namespace Everest.Routing
{
	public class RouteScanner : IRouteScanner
	{
		public ILogger<RouteScanner> Logger { get; }

		public RouteScanner(ILogger<RouteScanner> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public IEnumerable<RouteDescriptor> Scan(Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			var count = 0;

			Logger.LogTrace($"Scanning assembly for routes: {new { Assembly = assembly }}");
			foreach (var type in GetRestResourceTypes(assembly))
			{
				var routePrefixAttribute = GetAttributes<RoutePrefixAttribute>(type).FirstOrDefault();

				Logger.LogTrace($"Scanning type for routes: {new { Type = type }}");
				foreach (var method in GetRestRouteMethods(type))
				{
					var routeAttribute = GetAttributes<RestRouteAttribute>(method).FirstOrDefault();
					routePrefixAttribute = GetAttributes<RoutePrefixAttribute>(method).FirstOrDefault() ?? routePrefixAttribute;
					
					if (routeAttribute != null)
					{
						var action = (Func<HttpContext, Task>)method.CreateDelegate(typeof(Func<HttpContext, Task>), null);
						var routePrefix = routePrefixAttribute?.RoutePrefix ?? string.Empty;
						var route = new Route(routeAttribute.HttpMethod, $"{routePrefix}{routeAttribute.RoutePattern}");
						var endPoint = new EndPoint(type, method, action);
						var descriptor = new RouteDescriptor(route, endPoint);

						Logger.LogTrace($"Route found: {new { RoutePattern = route.Description, EndPoint = endPoint.Description }}");
						count++;

						yield return descriptor;
					}
				}
			}

			Logger.LogTrace($"Scan of assembly complete: {new { Assembly = assembly }}");
			Logger.LogTrace($"Total routes found: {new { Count = count }}");
		}

		private static IEnumerable<Type> GetRestResourceTypes(Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			foreach (var type in assembly
						 .GetTypes()
						 .Where(t => t.IsClass && t.IsDefined(typeof(RestResourceAttribute), false)))
			{
				yield return type;
			}
		}

		private static IEnumerable<MethodInfo> GetRestRouteMethods(Type type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			foreach (var method in type
						 .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
						 .Where(m => !m.IsAbstract && m.IsDefined(typeof(RestRouteAttribute), true)))
			{
				yield return method;
			}
		}

		private static IEnumerable<T> GetAttributes<T>(MethodInfo method)
			where T : Attribute
		{
			if (method == null)
				throw new ArgumentNullException(nameof(method));

			foreach (var attribute in method.GetCustomAttributes(typeof(T), false).OfType<T>())
			{
				yield return attribute;
			}
		}

		private static IEnumerable<T> GetAttributes<T>(Type type)
			where T : Attribute
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			foreach (var attribute in type.GetCustomAttributes(typeof(T), false).OfType<T>())
			{
				yield return attribute;
			}
		}
	}
}
