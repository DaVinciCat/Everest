using System;

namespace Everest.OpenApi.Annotations
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequestBodyAttribute : Attribute
    {
        public string[] MediaTypes { get; }
        
        public string Description { get; set; }

        public bool Required { get; set; }

        public RequestBodyAttribute(params string[] mediaTypes)
        {
            MediaTypes = mediaTypes;
        }
    }
}
