﻿using System;
using System.Linq;

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
            if (!exampleType.GetInterfaces().Contains(typeof(IOpenApiExampleProvider)))
            {
                throw new InvalidCastException($"Type {exampleType} does not implement {typeof(IOpenApiExampleProvider)}.");
            }

            ExampleType = exampleType;
            MediaType = mediaType;
        }
    }
}
