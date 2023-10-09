using System;

namespace Everest.OpenApi.Annotations
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequestBodyAttribute : Attribute
    {
        public string[] MimeTypes { get; set; }
        
        public string Description { get; set; }

        public bool Required { get; set; }

        public RequestBodyAttribute(params string[] mimeTypes)
        {
            MimeTypes = mimeTypes;
        }
    }
}
