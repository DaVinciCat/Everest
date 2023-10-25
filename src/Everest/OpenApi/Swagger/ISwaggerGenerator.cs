using Microsoft.OpenApi.Models;
using Everest.Routing;

namespace Everest.OpenApi.Swagger
{
    public interface ISwaggerGenerator
    {
        void Generate(OpenApiInfo info, RouteDescriptor[] routes);
    }
}
