using System.Linq;
using Everest.Routing;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Filters
{
    public class RestRouteDocumentFilter : OpenApiDocumentFilter
    {
        protected override void Apply(OpenApiDocumentContext context, RouteDescriptor descriptor)
        {
            if (!context.GetAttributes<RestRouteAttribute>(descriptor).Any())
            {
                return;
            }

            var item = new OpenApiPathItem();
            item.Operations.Add(context.GetOpenApiOperationType(descriptor), new OpenApiOperation
            {
                OperationId = descriptor.EndPoint.MethodInfo.Name
            });

            var document = context.Document;
            document.Paths.Add(context.GetOpenApiPathItemKey(descriptor), item);
        }
    }
}
