using Everest.OpenApi.Annotations;
using Everest.Routing;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Filters
{
    public class TagsDocumentFilter : OpenApiDocumentFilter
    {
        protected override void Apply(OpenApiDocumentContext context, RouteDescriptor descriptor)
        {
            var document = context.Document;
            if (!document.Paths.TryGetValue(descriptor.GetOpenApiPathItemKey(), out var item)) 
                return;

            if (!item.Operations.TryGetValue(descriptor.GetOpenApiOperationType(), out var operation))
                return;

            var attributes = descriptor.GetAttributes<TagsAttribute>();
            foreach (var attribute in attributes)
            {
                foreach (var name in attribute.Tags)
                {
                    if(!document.Tags.TryGetValue(name, out var tag))
                    {
                        tag = new OpenApiTag
                        {
                            Name = name
                        };

                        document.Tags.Add(tag);
                    }

                    operation.Tags.Add(tag);
                }
            }
        }
    }
}
