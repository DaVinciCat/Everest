using System;

namespace Everest.OpenApi.Annotations
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequestBodyAttribute : Attribute
    {
        public Type RequestBodyType { get; set; }

        public string[] MediaTypes { get; }
        
        public string Description { get; set; }

        public bool IsOptional { get; set; }

        public RequestBodyAttribute(Type requestBodyType, params string[] mediaTypes)
        {
            RequestBodyType = requestBodyType;
            MediaTypes = mediaTypes;
        }

        public RequestBodyAttribute(params string[] mediaTypes)
        {
            MediaTypes = mediaTypes;
        }
    }
}
