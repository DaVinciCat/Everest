using System;
using System.Collections.Generic;
using System.Linq;
using Everest.OpenApi.Annotations;
using Everest.Routing;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Filters
{
    public class RequestBodyExampleDocumentFilter : OpenApiDocumentFilter
    {
        public string DefaultExampleName { get; set; } = "Example";

        protected override void Apply(OpenApiDocument document, RouteDescriptor descriptor)
        {
            if (!document.Paths.TryGetValue(descriptor.GetOpenApiPathItemKey(), out var item))
                return;

            if (!item.Operations.TryGetValue(descriptor.GetOpenApiOperationType(), out var operation))
                return;

            var lookup = new Dictionary<IOpenApiExampleProvider, RequestBodyExampleAttribute>();
            var attributes = descriptor.GetAttributes<RequestBodyExampleAttribute>().ToArray();
            foreach (var attribute in attributes)
            {
                var provider = Activator.CreateInstance(attribute.ExampleType) as IOpenApiExampleProvider;
                if (provider == null)
                {
                    throw new InvalidCastException($"Type {attribute.ExampleType} does not implement {typeof(IOpenApiExampleProvider)}.");
                }

                lookup[provider] = attribute;
            }

            var groups = lookup.Keys.GroupBy(p => p.MimeType);
            foreach (var group in groups)
            {
                if (!operation.RequestBody.Content.TryGetValue(group.Key, out var content))
                {
                    continue;
                }

                if (group.Count() == 1)
                {
                    var provider = group.First();
                    content.Example = new OpenApiString(provider.GetExample());
                }
                else
                {
                    var index = 0;
                    foreach (var provider in group)
                    {
                        var attribute = lookup[provider];
                        var example = new OpenApiExample
                        {
                            Value = new OpenApiString(provider.GetExample()),
                            Summary = attribute.Summary,
                            Description = attribute.Description
                        };

                        var key = attribute.Name ?? $"{DefaultExampleName}{++index}";
                        content.Examples.Add(key, example);
                    }
                }
            }
        }
    }
}
