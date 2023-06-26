using System.Collections;
using System.Collections.Generic;

namespace Everest.Cors
{
	public class CorsPolicyCollection : IEnumerable<CorsPolicy>
	{
		private readonly Dictionary<string, CorsPolicy> policies = new();

		public void Add(CorsPolicy policy)
		{
			policies[policy.Origin] = policy;
		}

		public bool Remove(CorsPolicy policy)
		{
			return policies.Remove(policy.Origin);
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
