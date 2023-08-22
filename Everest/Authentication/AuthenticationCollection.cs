using System.Collections;
using System.Collections.Generic;

namespace Everest.Authentication
{
    public class AuthenticationCollection : IEnumerable<IAuthentication>
    {
        public IAuthentication this[string scheme]
        {
            get => authentications[scheme];
            set => authentications[scheme] = value;
        }

        private readonly Dictionary<string, IAuthentication> authentications = new();

        public bool Has(string scheme)
        {
            return authentications.ContainsKey(scheme);
        }

        public bool TryGet(string scheme, out IAuthentication corsPolicy)
        {
            return authentications.TryGetValue(scheme, out corsPolicy);
        }

        public void Add(IAuthentication authentication)
        {
            authentications[authentication.Scheme] = authentication;
        }

        public void Remove(IAuthentication authentication)
        {
            if (authentications.ContainsKey(authentication.Scheme))
            {
                authentications.Remove(authentication.Scheme);
            }
        }

        public void Remove(string scheme)
        {
            if (authentications.ContainsKey(scheme))
            {
                authentications.Remove(scheme);
            }
        }

        public void Clear()
        {
            authentications.Clear();
        }

        public IEnumerator<IAuthentication> GetEnumerator()
        {
            return authentications.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
