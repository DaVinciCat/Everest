using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Everest.Routing;

namespace Everest.Collections
{
	public interface IRouteCollection : IEnumerable<RouteDescriptor>
	{
		void Add(RouteDescriptor routeDescriptor);
	}

	public class RouteCollection : IRouteCollection
	{
		private readonly HashSet<RouteDescriptor> descriptors = new();

		public void Add(RouteDescriptor routeDescriptor)
		{
			if (descriptors.Any(o => o.Route.Description == routeDescriptor.Route.Description))
				throw new ArgumentException($"Duplicate route: {routeDescriptor.Route.Description}.");

			descriptors.Add(routeDescriptor);
		}

		#region IEnumerable

		public IEnumerator<RouteDescriptor> GetEnumerator()
		{
			return descriptors.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
