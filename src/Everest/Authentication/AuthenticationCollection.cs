using System.Collections;
using System.Collections.Generic;

namespace Everest.Authentication
{
    public class AuthenticationCollection : IEnumerable<string>
    {
        public IAuthentication this[string scheme]
        {
            get => authentications[scheme];
            set => authentications[scheme] = value;
        }

        private readonly Dictionary<string, IAuthentication> authentications = new Dictionary<string, IAuthentication>();

        public bool Has(string scheme)
        {
            return authentications.ContainsKey(scheme);
        }

        public bool TryGet(string scheme, out IAuthentication authentication)
        {
            return authentications.TryGetValue(scheme, out authentication);
        }

        public void Add(string scheme, IAuthentication authentication)
        {
            authentications[scheme] = authentication;
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

        public IEnumerator<string> GetEnumerator()
        {
            return authentications.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
