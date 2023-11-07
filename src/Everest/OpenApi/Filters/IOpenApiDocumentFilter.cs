using Everest.Routing;

namespace Everest.OpenApi.Filters
{
    public interface IOpenApiDocumentFilter
    {
        void Apply(OpenApiDocumentContext context, RouteDescriptor descriptor);
    }
}
