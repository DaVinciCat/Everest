using Everest.Rest;
using Microsoft.Extensions.DependencyInjection;

namespace Everest.SwaggerUi
{
    public static class RestServerBuilderExtensions
    {
        public static RestServerBuilder GenerateSwaggerUi(this RestServerBuilder builder)
        {
            var generator = builder.Services.GetRequiredService<ISwaggerUiGenerator>();
            generator.Generate();

            return builder;
        }
    }
}
