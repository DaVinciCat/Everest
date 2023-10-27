using Everest.Routing;
using Microsoft.OpenApi.Models;
using System.Linq;
using Everest.OpenApi.Annotations;
using System.Collections.Generic;
using System;
using Everest.OpenApi.Examples;
using Microsoft.OpenApi.Any;

namespace Everest.OpenApi.Filters
{
    public class ResponseExampleDocumentFilter : OpenApiDocumentFilter
    {
        public string DefaultExampleName { get; set; } = "Example";

        protected override void Apply(OpenApiDocumentContext context, RouteDescriptor descriptor)
        {
            var document = context.Document;
            if (!document.Paths.TryGetValue(context.GetOpenApiPathItemKey(descriptor), out var item))
                return;

            if (!item.Operations.TryGetValue(context.GetOpenApiOperationType(descriptor), out var operation))
                return;

            var lookup = new Dictionary<ResponseExampleAttribute, IOpenApiExampleProvider>();
            var attributes = context.GetAttributes<ResponseExampleAttribute>(descriptor).ToArray();
            foreach (var attribute in attributes)
            {
                var provider = Activator.CreateInstance(attribute.ExampleType) as IOpenApiExampleProvider;
                if (provider == null)
                {
                    throw new InvalidCastException($"Type {attribute.ExampleType} does not implement {typeof(IOpenApiExampleProvider)}.");
                }

                lookup[attribute] = provider;
            }

            var groups = lookup.Keys.GroupBy(p => p.StatusCode).ToArray();
            foreach (var group in groups)
            {
                var code = ((int)group.Key).ToString();
                if (!operation.Responses.TryGetValue(code, out var content))
                {
                    continue;
                }

                var groups2 = group.GroupBy(p => p.MediaType).ToArray();
                foreach (var group2 in groups2)
                {
                    if(!content.Content.TryGetValue(group2.Key, out var media))
                        continue;

                    if (group2.Count() == 1)
                    {
                        var provider = lookup[group2.First()];
                        media.Example = new OpenApiString(provider.GetExample());
                    }
                    else
                    {
                        var index = 0;
                        foreach (var attribute in group2)
                        {
                            var provider = lookup[attribute];
                            var example = new OpenApiExample
                            {
                                Value = new OpenApiString(provider.GetExample()),
                                Summary = attribute.Summary,
                                Description = attribute.Description
                            };

                            var key = attribute.Name ?? $"{DefaultExampleName}{++index}";
                            media.Examples.Add(key, example);
                        }
                    }
                }
            }
        }
    }
}
