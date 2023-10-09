using System;
using System.Linq;

namespace Everest.OpenApi.Annotations
{
    [AttributeUsage( AttributeTargets.Method, AllowMultiple = true)]
    public class RequestBodyExampleAttribute : Attribute
    {
        public string Name { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public Type ExampleType { get; }

        public RequestBodyExampleAttribute(Type exampleType)
        {
            if (!exampleType.GetInterfaces().Contains(typeof(IOpenApiExampleProvider)))
            {
                throw new InvalidCastException($"Type {exampleType} does not implement {typeof(IOpenApiExampleProvider)}.");
            }

            ExampleType = exampleType;
        }
    }
}
