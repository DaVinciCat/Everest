using System.Linq;
using Everest.OpenApi.Annotations;
using Everest.Routing;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Filters
{
    public interface IOpenApiDocumentFilter
    {
        void Apply(OpenApiDocument document, RouteDescriptor descriptor);
    }

    public abstract class OpenApiDocumentFilter : IOpenApiDocumentFilter
    {
        void IOpenApiDocumentFilter.Apply(OpenApiDocument document, RouteDescriptor descriptor)
        {
            var ignore = descriptor.GetAttributes<OpenApiIgnoreAttribute>().Any();
            if (ignore)
            {
                return;
            }

            Apply(document, descriptor);
        }

        protected abstract void Apply(OpenApiDocument document, RouteDescriptor descriptor);
    }
}
