﻿using System.Threading.Tasks;
using Everest.Http;
using Everest.Utils;
using Microsoft.Extensions.Logging;

namespace Everest.Routing
{
    public interface IRouter
	{
		RouteDescriptor[] Routes { get; }

		void RegisterRoute(RouteDescriptor descriptor);

		Task<bool> TryRouteAsync(IHttpContext context);
	}

    public static partial class HasLoggerExtensions
    {
        public static ILogger GetLogger(this IRouter instance) => (instance as IHasLogger)?.Logger;
    }
}
