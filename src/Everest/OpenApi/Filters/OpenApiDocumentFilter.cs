﻿using System.Linq;
using Everest.OpenApi.Annotations;
using Everest.Routing;

namespace Everest.OpenApi.Filters
{
    public abstract class OpenApiDocumentFilter : IOpenApiDocumentFilter
    {
        void IOpenApiDocumentFilter.Apply(OpenApiDocumentContext context, RouteDescriptor descriptor)
        {
            var ignore = context.GetAttributes<OpenApiIgnoreAttribute>(descriptor).Any();
            if (ignore)
            {
                return;
            }

            Apply(context, descriptor);
        }

        protected abstract void Apply(OpenApiDocumentContext context, RouteDescriptor descriptor);
    }
}
