using System.Collections.Generic;
using Everest.Routing;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi
{
    public interface IOpenApiPathParametersGenerator
    {
        IEnumerable<OpenApiParameter> GetParameters(RouteDescriptor descriptor);
    }
}
