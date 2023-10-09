using System;
using System.Net;

namespace Everest.OpenApi.Annotations
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ResponseAttribute : Attribute
    {
        public string[] MimeTypes { get; set; }

        public string Description { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public ResponseAttribute(HttpStatusCode statusCode, params string[] mimeTypes)
        {
            StatusCode = statusCode;
            MimeTypes = mimeTypes;
        }
    }
}
