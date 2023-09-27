using System;
using System.Collections;
using System.Collections.Generic;

namespace Everest.Routing
{
    public class RouteSegmentParserCollection : IEnumerable<string>
    {
        public Func<string, IRouteSegmentParser> this[string pattern]
        {
            get => parsers[pattern];
            set => parsers[pattern] = value;
        }

        private readonly Dictionary<string, Func<string, IRouteSegmentParser>> parsers = new Dictionary<string, Func<string, IRouteSegmentParser>>();

        public bool Has(string pattern)
        {
            return parsers.ContainsKey(pattern);
        }

        public bool TryGet(string pattern, out Func<string, IRouteSegmentParser> parser)
        {
            return parsers.TryGetValue(pattern, out parser);
        }

        public void Add(string pattern, Func<string, IRouteSegmentParser> factory)
        {
            parsers[pattern] = factory;
        }
        
        public void Remove(string pattern)
        {
            if (parsers.ContainsKey(pattern))
            {
                parsers.Remove(pattern);
            }
        }
        public void Clear()
        {
            parsers.Clear();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return parsers.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
