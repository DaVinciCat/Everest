using System;
using Everest.OpenApi.Examples;

namespace Everest.OpenApi.Annotations
{
    [AttributeUsage( AttributeTargets.Method, AllowMultiple = true)]
    public class RequestBodyExampleAttribute : Attribute
    {
        public Type ExampleType { get; }

        public string MediaType { get; }

        public string Name { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }
        
        public RequestBodyExampleAttribute(Type exampleType, string mediaType)
        {
            if (exampleType.GetInterface(nameof(IOpenApiExampleProvider)) == null)
            {
                throw new InvalidCastException($"Type {exampleType} does not implement {nameof(IOpenApiExampleProvider)}.");
            }
            
            ExampleType = exampleType;
            MediaType = mediaType;
        }
    }
}
