using Everest.Rest;
using Microsoft.Extensions.DependencyInjection;

namespace Everest.Compression
{
    public static class RestServerBuilderExtensions
    {
        public static RestServerBuilder UseResponseCompressionMiddleware(this RestServerBuilder builder)
        {
            var responseCompressor = builder.Services.GetRequiredService<IResponseCompressor>();
            builder.Middleware.Add(new ResponseCompressionMiddleware(responseCompressor));
            return builder;
        }
    }
}
