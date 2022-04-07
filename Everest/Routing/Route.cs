using System;
using Everest.Http;

namespace Everest.Routing
{
	public class Route : IEquatable<Route>
	{
		private readonly string route;

		public Route(string httpMethod, string endpoint)
		{
			route = $"{httpMethod} {endpoint.TrimEnd('/')}";
		}

		public Route(HttpRequest request)
			: this(request.HttpMethod, request.EndPoint)
		{
			
		}

		public static implicit operator string(Route route)
		{
			return route.ToString();
		}

		public override string ToString()
		{
			return route;
		}

		public bool Equals(Route other)
		{
			if (ReferenceEquals(null, other)) 
				return false;

			if (ReferenceEquals(this, other)) 
				return true;

			return route == other.route;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) 
				return false;

			if (ReferenceEquals(this, obj)) 
				return true;

			if (obj.GetType() != GetType())
				return false;

			return Equals((Route)obj);
		}

		public override int GetHashCode()
		{
			return route != null ? route.GetHashCode() : 0;
		}
	}
}
