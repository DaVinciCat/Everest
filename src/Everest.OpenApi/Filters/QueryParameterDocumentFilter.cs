using Everest.Routing;
using Microsoft.OpenApi.Models;
using System.Linq;
using Everest.OpenApi.Annotations;

namespace Everest.OpenApi.Filters
{
    public class QueryParameterDocumentFilter : OpenApiDocumentFilter
    {
        protected override void Apply(OpenApiDocumentContext context, RouteDescriptor descriptor)
        {
            var document = context.Document;
            if (!document.Paths.TryGetValue(context.GetOpenApiPathItemKey(descriptor), out var item))
                return;

            if (!item.Operations.TryGetValue(context.GetOpenApiOperationType(descriptor), out var operation))
                return;

            var attributes = context.GetAttributes<QueryParameterAttribute>(descriptor).ToArray();
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
