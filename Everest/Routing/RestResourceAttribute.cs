using System;
namespace Everest.Routing
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class RestResourceAttribute : Attribute
	{
		public string RoutePrefix { get; }

		public RestResourceAttribute(string routePrefix = null)
		{
			RoutePrefix = routePrefix;
		}
	}
}
