﻿using System.Linq;
using Everest.Routing;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Filters
{
    public class RestRouteDocumentFilter : OpenApiDocumentFilter
    {
        protected override void Apply(OpenApiDocumentContext context, RouteDescriptor descriptor)
        {
            var hasAttribute = descriptor.GetAttributes<RestRouteAttribute>().Any();
            if (!hasAttribute)
            {
                return;
            }

            var item = new OpenApiPathItem();
            item.Operations.Add(descriptor.GetOpenApiOperationType(), new OpenApiOperation
            {
                OperationId = descriptor.EndPoint.MethodInfo.Name
            });

            var document = context.Document;
            document.Paths.Add(descriptor.GetOpenApiPathItemKey(), item);
        }
    }
}
