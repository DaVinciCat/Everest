using System;
using System.Collections.Generic;
using System.Linq;
using Everest.OpenApi.Annotations;
using Everest.OpenApi.Examples;
using Everest.Routing;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Filters
{
    public class RequestBodyExampleDocumentFilter : OpenApiDocumentFilter
    {
        public string DefaultExampleName { get; set; } = "Example";

        protected override void Apply(OpenApiDocumentContext context, RouteDescriptor descriptor)
        {
            var document = context.Document;
            if (!document.Paths.TryGetValue(context.GetOpenApiPathItemKey(descriptor), out var item))
                return;

            if (!item.Operations.TryGetValue(context.GetOpenApiOperationType(descriptor), out var operation))
                return;

            var lookup = new Dictionary<RequestBodyExampleAttribute, IOpenApiExampleProvider>();
            var attributes = context.GetAttributes<RequestBodyExampleAttribute>(descriptor).ToArray();
            foreach (var attribute in attributes)
            {
                var provider = Activator.CreateInstance(attribute.ExampleType) as IOpenApiExampleProvider;
                if (provider == null)
                {
                    throw new InvalidCastException($"Type {attribute.ExampleType} does not implement {nameof(IOpenApiExampleProvider)}.");
                }

                lookup[attribute] = provider;
            }

            var groups = lookup.Keys.GroupBy(p => p.MediaType).ToArray();
            foreach (var group in groups)
            {
                if (!operation.RequestBody.Content.TryGetValue(group.Key, out var content))
                {
                    continue;
                }

                if (group.Count() == 1)
                {
                    var provider = lookup[group.First()];
                    content.Example = new OpenApiString(provider.GetExample());
                }
                else
                {
                    var index = 0;
                    foreach (var attribute in group)
                    {
                        var provider = lookup[attribute];
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
