using System;
using System.Net;

namespace Everest.OpenApi.Annotations
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ResponseAttribute : Attribute
    {
        public HttpStatusCode StatusCode { get; }

        public string[] MediaTypes { get; }

        public string Description { get; set; }
        
        public ResponseAttribute(HttpStatusCode statusCode, params string[] mimeTypes)
        {
            StatusCode = statusCode;
            MediaTypes = mimeTypes;
        }
    }
}
