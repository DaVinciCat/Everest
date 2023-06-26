using System;
using Everest.Collections;
using Everest.Http;

namespace Everest.Routing
{
	public interface IRouteDescriptorFeature
	{
		RouteDescriptor RouteDescriptor { get; }
	}

	public class RouteDescriptorFeature : IRouteDescriptorFeature
	{
		public RouteDescriptor RouteDescriptor { get; }

		public RouteDescriptorFeature(RouteDescriptor descriptor)
		{
			RouteDescriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
		}
	}

	public static class HttpContextRouteDescriptorFeatureExtensions
	{
		public static RouteDescriptor GetRouteDescriptor(this HttpContext context)
		{
			if (context == null) 
				throw new ArgumentNullException(nameof(context));

			return context.Features.Get<IRouteDescriptorFeature>()?.RouteDescriptor ?? throw new ArgumentNullException(nameof(IRouteDescriptorFeature));
		}

		public static bool TryGetRouteDescriptor(this HttpContext context, out RouteDescriptor descriptor)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			descriptor = context.Features.Get<IRouteDescriptorFeature>()?.RouteDescriptor;
			return descriptor != null;
		}
	}
}
