using System;
using Microsoft.Extensions.DependencyInjection;

namespace Everest.EndPoints
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddEndPointInvoker(this IServiceCollection services, Func<IServiceProvider, IEndPointInvoker> builder)
        {
            services.AddSingleton(builder);
            return services;
        }
    }
}
