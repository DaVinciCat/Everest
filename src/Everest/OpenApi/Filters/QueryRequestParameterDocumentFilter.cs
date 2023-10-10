using Everest.Routing;
using Microsoft.OpenApi.Models;
using System.Linq;
using Everest.OpenApi.Annotations;

namespace Everest.OpenApi.Filters
{
    public class QueryRequestParameterDocumentFilter : OpenApiDocumentFilter
    {
        protected override void Apply(OpenApiDocumentContext context, RouteDescriptor descriptor)
        {
            var document = context.Document;
            if (!document.Paths.TryGetValue(descriptor.GetOpenApiPathItemKey(), out var item))
                return;

            if (!item.Operations.TryGetValue(descriptor.GetOpenApiOperationType(), out var operation))
                return;

            var attributes = descriptor.GetAttributes<QueryRequestParameterAttribute>().ToArray();
            foreach (var attribute in attributes)
            {
                var parameter = new OpenApiParameter
                {
                    In = ParameterLocation.Query,
                    Name = attribute.Name,
                    Description = attribute.Description,
                    Required = !attribute.IsOptional,
                    Deprecated = attribute.Deprecated,
                    AllowEmptyValue = attribute.AllowEmptyValue,
                    Schema = context.SchemaGenerator.GetSchema(attribute.Type)
                };

                operation.Parameters.Add(parameter);
            }
        }
    }
}
