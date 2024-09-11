using Everest.Utils;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Reflection;

namespace Everest.Routing
{
	public interface IRouteScanner
	{
		IEnumerable<RouteDescriptor> Scan(Assembly assembly);
	}

    public static partial class HasLoggerExtensions
    {
        public static ILogger GetLogger(this IRouteScanner instance) => (instance as IHasLogger)?.Logger;
    }
}
