﻿using System.Linq;
using Everest.OpenApi.Annotations;
using Everest.Routing;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Filters
{
    public class ResponseDocumentFilter : OpenApiDocumentFilter
    {
        protected override void Apply(OpenApiDocumentContext context, RouteDescriptor descriptor)
        {
            var document = context.Document;
            if (!document.Paths.TryGetValue(descriptor.GetOpenApiPathItemKey(), out var item))
                return;

            if (!item.Operations.TryGetValue(descriptor.GetOpenApiOperationType(), out var operation))
                return;

            var attributes = descriptor.GetAttributes<ResponseAttribute>().ToArray();
            foreach (var attribute in attributes)
            {
                var response = new OpenApiResponse
                {
                    Description = attribute.Description
                };

                foreach (var media in attribute.MediaTypes)
                {
                    var content = new OpenApiMediaType();
                    if (attribute.ResponseType != null)
                    {
                        var schema = context.SchemaGenerator.GetSchema(attribute.ResponseType);
                        content.Schema = schema;
                    }

                    response.Content.Add(media, content);
                }
                
                var key = attribute.StatusCode.ToString();
                operation.Responses.Add(key, response);
            }
        }
    }
}