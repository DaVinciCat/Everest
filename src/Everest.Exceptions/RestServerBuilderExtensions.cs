using Everest.Rest;
using Microsoft.Extensions.DependencyInjection;

namespace Everest.Exceptions
{
    public static class RestServerBuilderExtensions
    {
        public static RestServerBuilder UseExceptionHandlerMiddleware(this RestServerBuilder builder)
        {
            var exceptionHandler = builder.Services.GetRequiredService<IExceptionHandler>();
            builder.Middleware.Add(new ExceptionHandlerMiddleware(exceptionHandler));
            return builder;
        }
    }
}
