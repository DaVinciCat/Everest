using Microsoft.Extensions.DependencyInjection;

namespace Everest.WebSockets
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddWebSocketRequestHandler<THandler>(this IServiceCollection services)
            where THandler : class, IWebSocketRequestHandler
        {
            services.AddSingleton<THandler>();
            return services;
        }
    }
}
