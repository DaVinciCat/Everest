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
            if (!document.Paths.TryGetValue(context.GetOpenApiPathItemKey(descriptor), out var item)) 
                return;

            if (!item.Operations.TryGetValue(context.GetOpenApiOperationType(descriptor), out var operation))
                return;

            var attributes = context.GetAttributes<TagsAttribute>(descriptor);
            foreach (var attribute in attributes)
            {
                foreach (var name in attribute.Tags)
                {
                    if(!document.Tags.TryGetTag(name, out var tag))
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
