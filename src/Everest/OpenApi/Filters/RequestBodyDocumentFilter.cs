using System.Linq;
using Everest.OpenApi.Annotations;
using Everest.Routing;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Filters
{
    public class RequestBodyDocumentFilter : OpenApiDocumentFilter
    {
        protected override void Apply(OpenApiDocumentContext context, RouteDescriptor descriptor)
        {
            var document = context.Document;
            if (!document.Paths.TryGetValue(context.GetOpenApiPathItemKey(descriptor), out var item))
                return;

            if (!item.Operations.TryGetValue(context.GetOpenApiOperationType(descriptor), out var operation))
                return;

            var attributes = context.GetAttributes<RequestBodyAttribute>(descriptor).ToArray();
            if (attributes.Length == 0)
                return;
            var attribute = attributes.First();

            var body = new OpenApiRequestBody
            {
                Description = attribute.Description,
                Required = !attribute.IsOptional
            };

            foreach (var media in attribute.MediaTypes)
            {
                var content = new OpenApiMediaType();

                if (attribute.RequestBodyType != null)
                {
                    var schema = context.SchemaGenerator.GetSchema(attribute.RequestBodyType); 
                    content.Schema = schema;
                }

                body.Content.Add(media, content);
            }

            operation.RequestBody = body;
        }
    }
}
