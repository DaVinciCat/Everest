using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Reflection;
using Everest.Common.Logging;

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
