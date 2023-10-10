using System;
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

        public ResponseExampleAttribute(HttpStatusCode statusCode, string mediaType, Type exampleType)
        {
            if (exampleType.GetInterface(nameof(IOpenApiExampleProvider)) == null)
            {
                throw new InvalidCastException($"Type {exampleType} does not implement {nameof(IOpenApiExampleProvider)}.");
            }

            MediaType = mediaType;
            StatusCode = statusCode;
            ExampleType = exampleType;
        }
    }
}
