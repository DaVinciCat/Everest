using System;
using System.Net;

namespace Everest.OpenApi.Annotations
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class ResponseAttribute : Attribute
    {
        public HttpStatusCode StatusCode { get; }

        public string[] MediaTypes { get; }

        public Type ResponseType { get; }

        public string Description { get; set; }
        
        public ResponseAttribute(HttpStatusCode statusCode, params string[] mediaTypes)
        {
            StatusCode = statusCode;
            MediaTypes = mediaTypes;
        }

        public ResponseAttribute(HttpStatusCode statusCode, Type responseType, params string[] mediaTypes)
        {
            StatusCode = statusCode;
            ResponseType = responseType;
            MediaTypes = mediaTypes;
        }
    }
}
