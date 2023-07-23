using System.Collections;
using System.Collections.Generic;

namespace Everest.Cors
{
	public class CorsPolicyCollection : IEnumerable<CorsPolicy>
	{
		private readonly Dictionary<string, CorsPolicy> policies = new();

		public bool TryGetCorsPolicy(string origin, out CorsPolicy policy)
		{
			return policies.TryGetValue(origin, out policy);
		}

		public void Add(CorsPolicy policy)
		{
			policies[policy.Origin] = policy;
		}

		public bool Remove(CorsPolicy policy)
		{
			return policies.Remove(policy.Origin);
		}

		public void Clear()
		{
			policies.Clear();
		}

		public IEnumerator<CorsPolicy> GetEnumerator()
		{
			return policies.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
