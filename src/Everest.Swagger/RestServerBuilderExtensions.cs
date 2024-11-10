using Everest.Rest;
using Everest.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Everest.Swagger
{
    public static class RestServerBuilderExtensions
    {
        public static RestServerBuilder GenerateSwaggerEndPoint(this RestServerBuilder builder)
        {
            var generator = builder.Services.GetRequiredService<ISwaggerEndPointGenerator>();
            var router = builder.Services.GetRequiredService<IRouter>();

            generator.Generate(router.Routes);

            return builder;
        }
    }
}
