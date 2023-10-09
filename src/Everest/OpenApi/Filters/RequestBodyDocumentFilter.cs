using System.Linq;
using Everest.OpenApi.Annotations;
using Everest.Routing;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Filters
{
    public class RequestBodyDocumentFilter : OpenApiDocumentFilter
    {
        protected override void Apply(OpenApiDocument document, RouteDescriptor descriptor)
        {
            if (!document.Paths.TryGetValue(descriptor.GetOpenApiPathItemKey(), out var item))
                return;

            if (!item.Operations.TryGetValue(descriptor.GetOpenApiOperationType(), out var operation))
                return;

            var attributes = descriptor.GetAttributes<RequestBodyAttribute>().ToArray();
            if (attributes.Length == 0)
                return;
            var attribute = attributes.First();

            var body = new OpenApiRequestBody
            {
                Description = attribute.Description,
                Required = attribute.Required
            };

            foreach (var mime in attribute.MimeTypes)
            {
                var content = new OpenApiMediaType();
                body.Content.Add(mime, content);
            }

            operation.RequestBody = body;
        }
    }
}
