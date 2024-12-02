using Microsoft.OpenApi.Models;

namespace Everest.OpenApi.Parameters
{
    public interface IOpenApiPathParameterProvider
    {
        bool TryGetParameter(string segment, out OpenApiParameter parameter);
    }
}
