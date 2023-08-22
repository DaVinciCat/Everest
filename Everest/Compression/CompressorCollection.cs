using Everest.Authentication;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Everest.Compression
{
    public class CompressorCollection : IEnumerable<string>
    {
        public Func<Stream, Stream> this[string encoding]
        {
            get => compressors[encoding];
            set => compressors[encoding] = value;
        }

        private readonly Dictionary<string, Func<Stream, Stream>> compressors = new();

        public bool Has(string encoding)
        {
            return compressors.ContainsKey(encoding);
        }

        public bool TryGet(string encoding, out Func<Stream, Stream> compressor)
        {
            return compressors.TryGetValue(encoding, out compressor);
        }

        public void Add(string encoding, Func<Stream, Stream> compressor)
        {
            compressors[encoding] = compressor;
        }
        
        public void Remove(string encoding)
        {
            if (compressors.ContainsKey(encoding))
            {
                compressors.Remove(encoding);
            }
        }

        public void Clear()
        {
            compressors.Clear();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return compressors.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
