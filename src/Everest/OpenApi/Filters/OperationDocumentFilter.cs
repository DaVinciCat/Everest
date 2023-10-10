using Everest.Routing;
using System.Linq;
using Everest.OpenApi.Annotations;

namespace Everest.OpenApi.Filters
{
    internal class OperationDocumentFilter : IOpenApiDocumentFilter
    {
        public void Apply(OpenApiDocumentContext context, RouteDescriptor descriptor)
        {
            var document = context.Document;
            if (!document.Paths.TryGetValue(descriptor.GetOpenApiPathItemKey(), out var item))
                return;

            if (!item.Operations.TryGetValue(descriptor.GetOpenApiOperationType(), out var operation))
                return;

            var attributes = descriptor.GetAttributes<OperationAttribute>().ToArray();
            if (attributes.Length == 0)
                return;
            var attribute = attributes.First();

            if (!string.IsNullOrEmpty(attribute.OperationId))
            {
                operation.OperationId = attribute.OperationId;
            }

            operation.Summary = attribute.Summary;
            operation.Description = attribute.Description;
            operation.Deprecated = attribute.Deprecated;
        }
    }
}
