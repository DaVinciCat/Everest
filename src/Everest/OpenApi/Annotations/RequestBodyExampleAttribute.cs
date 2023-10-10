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
        
        public RequestBodyExampleAttribute(string mediaType, Type exampleType)
        {
            if (exampleType.GetInterface(nameof(IOpenApiExampleProvider)) == null)
            {
                throw new InvalidCastException($"Type {exampleType} does not implement {nameof(IOpenApiExampleProvider)}.");
            }

            MediaType = mediaType;
            ExampleType = exampleType;
        }
    }
}
