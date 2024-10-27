using Everest.Core.Rest;
using Microsoft.Extensions.DependencyInjection;

namespace Everest.SwaggerUi
{
    public static class RestServerBuilderExtensions
    {
        public static RestServerBuilder UseSwaggerUi(this RestServerBuilder builder)
        {
            var generator = builder.Services.GetRequiredService<ISwaggerUiGenerator>();
            generator.Generate();

            return builder;
        }
    }
}
