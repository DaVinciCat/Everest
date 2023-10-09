using System;
using System.Linq;
using System.Net;

namespace Everest.OpenApi.Annotations
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ResponseExampleAttribute : Attribute
    {
        public HttpStatusCode StatusCode { get; }
        
        public Type ExampleType { get; }
        
        public string MediaType { get; }

        public string Name { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public ResponseExampleAttribute(HttpStatusCode statusCode, Type exampleType, string mediaType)
        {
            if (!exampleType.GetInterfaces().Contains(typeof(IOpenApiExampleProvider)))
            {
                throw new InvalidCastException($"Type {exampleType} does not implement {typeof(IOpenApiExampleProvider)}.");
            }

            StatusCode = statusCode;
            ExampleType = exampleType;
            MediaType = mediaType;
        }
    }
}
