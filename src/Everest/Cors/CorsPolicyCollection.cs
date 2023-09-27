using System.Collections;
using System.Collections.Generic;

namespace Everest.Cors
{
    public class CorsPolicyCollection : IEnumerable<CorsPolicy>
    {
        public CorsPolicy this[string origin]
        {
            get => policies[origin];
            set => policies[origin] = value;
        }

        private readonly Dictionary<string, CorsPolicy> policies = new Dictionary<string, CorsPolicy>();

        public bool Has(string origin)
        {
            return policies.ContainsKey(origin);
        }

        public bool TryGet(string origin, out CorsPolicy policy)
        {
            return policies.TryGetValue(origin, out policy);
        }

        public void Add(CorsPolicy policy)
        {
            policies[policy.Origin] = policy;
        }

        public void Remove(CorsPolicy policy)
        {
            if (policies.ContainsKey(policy.Origin))
            {
                policies.Remove(policy.Origin);
            }
        }

        public void Remove(string origin)
        {
            if (policies.ContainsKey(origin))
            {
                policies.Remove(origin);
            }
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
