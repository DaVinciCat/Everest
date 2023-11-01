using System;
using System.Net;
using Everest.OpenApi.Examples;

namespace Everest.OpenApi.Annotations
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
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
            if (exampleType.GetInterface(nameof(IOpenApiExampleProvider)) == null)
            {
                throw new InvalidCastException($"Type {exampleType} does not implement {nameof(IOpenApiExampleProvider)}.");
            }
            
            StatusCode = statusCode;
            ExampleType = exampleType;
            MediaType = mediaType;
        }
    }
}
