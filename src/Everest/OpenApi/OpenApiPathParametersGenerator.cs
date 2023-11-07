using System;
using System.Collections.Generic;
using System.Linq;
using Everest.OpenApi.Parameters;
using Everest.Routing;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi
{
    public class OpenApiPathParametersGenerator : IOpenApiPathParametersGenerator
    {
        public string[] Delimiters { get; set; } = { "/" };

        public IList<IOpenApiPathParameterProvider> ParameterProviders { get; }
        
        public OpenApiPathParametersGenerator(Func<Type, OpenApiSchema> getSchema)
        {
            if (getSchema == null)
                throw new ArgumentNullException(nameof(getSchema));

            ParameterProviders = new List<IOpenApiPathParameterProvider>
            {
                new StringPathParameterProvider(getSchema),
                new IntPathParameterProvider(getSchema),
                new DoublePathParameterProvider(getSchema),
                new FloatPathParameterProvider(getSchema),
                new BoolPathParameterProvider(getSchema),
                new GuidPathParameterProvider(getSchema),
                new DateTimePathParameterProvider(getSchema)
            };
        }

        public IEnumerable<OpenApiParameter> GetParameters(RouteDescriptor descriptor)
        {
            var segments = descriptor.Route.RoutePattern.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries).ToArray();
            foreach (var segment in segments)
            {
                foreach (var provider in ParameterProviders)
                {
                    if (provider.TryGetParameter(segment, out var parameter))
                    {
                        yield return parameter;
                        break;
                    }
                }
            }
        }
    }
}
