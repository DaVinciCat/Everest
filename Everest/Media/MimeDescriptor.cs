using System;
using System.Net.Mime;

namespace Everest.Media
{
    public class MimeDescriptor
    {
        public ContentType ContentType { get; }

        public ContentDisposition ContentDisposition { get; }

        public MimeDescriptor(ContentType contentType, ContentDisposition contentDisposition)
        {
            ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
            ContentDisposition = contentDisposition ?? throw new ArgumentNullException(nameof(contentDisposition));
        }

        public MimeDescriptor(ContentType contentType)
            : this(contentType, new ContentDisposition("inline"))
        {

        }

        public MimeDescriptor(string contentType)
            : this(new ContentType(contentType), new ContentDisposition("inline"))
        {

        }

        public MimeDescriptor(string contentType, string contentDisposition)
            : this(new ContentType(contentType), new ContentDisposition(contentDisposition))
        {

        }
    }
}
