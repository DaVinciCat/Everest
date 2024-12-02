using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;

namespace Everest.Mime
{
    public class ContentTypeCollection : IEnumerable<ContentType>
    {
        public ContentType this[string mediaType]
        {
            get => contentTypes[mediaType];
            set => contentTypes[mediaType] = value;
        }

        private readonly Dictionary<string, ContentType> contentTypes = new Dictionary<string, ContentType>();

        public bool Has(string mediaType)
        {
            if (mediaType == null)
                return false;

            return contentTypes.ContainsKey(mediaType);
        }

        public bool TryGet(string mediaType, out ContentType contentType)
        {
            return contentTypes.TryGetValue(mediaType, out contentType);
        }

        public void Add(ContentType contentType)
        {
            contentTypes[contentType.MediaType] = contentType;
        }

        public void Add(string mediaType)
        {
            contentTypes[mediaType] = new ContentType(mediaType);
        }

        public void Remove(ContentType contentType)
        {
            if (contentTypes.ContainsKey(contentType.MediaType))
            {
                contentTypes.Remove(contentType.MediaType);
            }
        }

        public void Remove(string mediaType)
        {
            if (contentTypes.ContainsKey(mediaType))
            {
                contentTypes.Remove(mediaType);
            }
        }

        public void Clear()
        {
            contentTypes.Clear();
        }

        public IEnumerator<ContentType> GetEnumerator()
        {
            return contentTypes.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
