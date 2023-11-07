using Everest.Routing;
using System.Linq;
using Everest.OpenApi.Annotations;

namespace Everest.OpenApi.Filters
{
    public class PathParameterDocumentFilter : OpenApiDocumentFilter
    {
        protected override void Apply(OpenApiDocumentContext context, RouteDescriptor descriptor)
        {
            var document = context.Document;
            if (!document.Paths.TryGetValue(context.GetOpenApiPathItemKey(descriptor), out var item))
                return;

            if (!item.Operations.TryGetValue(context.GetOpenApiOperationType(descriptor), out var operation))
                return;

            var parameters = context.PathParametersGenerator.GetParameters(descriptor).ToArray();
            foreach (var parameter in parameters)
            {
                operation.Parameters.Add(parameter);
            }

            var attributes = context.GetAttributes<PathParameterAttribute>(descriptor).ToArray();
            foreach (var attribute in attributes)
            {
                if (operation.Parameters.TryGetParameter(attribute.Name, out var parameter))
                {
                    parameter.Description = attribute.Description;
                }
            }
        }
    }
}