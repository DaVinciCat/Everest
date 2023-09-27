using System.Collections;
using System.Collections.Generic;

namespace Everest.Mime
{
    public class MimeTypeCollection : IEnumerable<MimeType>
    {
        public MimeType this[string fileExtension]
        {
            get => mimeTypes[fileExtension];
            set => mimeTypes[fileExtension] = value;
        }

        private readonly Dictionary<string, MimeType> mimeTypes = new();

        public bool Has(string fileExtension)
        {
            return mimeTypes.ContainsKey(fileExtension);
        }

        public bool TryGet(string fileExtension, out MimeType mimeType)
        {
            return mimeTypes.TryGetValue(fileExtension, out mimeType);
        }

        public void Add(MimeType mimeType)
        {
            mimeTypes[mimeType.FileExtension] = mimeType;
        }

        public void Add(string fileExtension, string contentType, bool isBinary)
        {
            mimeTypes[fileExtension] = new MimeType(fileExtension, contentType, isBinary);
        }

        public void Remove(MimeType mimeType)
        {
            if (mimeTypes.ContainsKey(mimeType.FileExtension))
            {
                mimeTypes.Remove(mimeType.FileExtension);
            }
        }

        public void Remove(string fileExtension)
        {
            if (mimeTypes.ContainsKey(fileExtension))
            {
                mimeTypes.Remove(fileExtension);
            }
        }

        public void Clear()
        {
            mimeTypes.Clear();
        }

        public IEnumerator<MimeType> GetEnumerator()
        {
            return mimeTypes.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
