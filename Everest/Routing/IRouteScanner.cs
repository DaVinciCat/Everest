using System.Collections.Generic;
using System.Reflection;

namespace Everest.Routing
{
	public interface IRouteScanner
	{
		IEnumerable<RouteDescriptor> Scan(Assembly assembly);
	}
}
