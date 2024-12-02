using Microsoft.Extensions.DependencyInjection;
using System;

namespace Everest.Mime
{
    public static class RestServerBuilderExtensions
    {
        public static IServiceCollection AddMimeTypesProvider(this IServiceCollection services, Func<IServiceProvider, IMimeTypesProvider> builder)
        {
            services.AddSingleton(builder);
            return services;
        }

        public static IServiceCollection AddMimeTypesProvider(this IServiceCollection services, Action<MimeTypesProviderConfigurator> configurator)
        {
            services.AddSingleton<IMimeTypesProvider>(provider =>
            {
                var mimeTypesProvider = new MimeTypesProvider();
                configurator(new MimeTypesProviderConfigurator(mimeTypesProvider, provider));

                return mimeTypesProvider;
            });

            return services;
        }
    }
}
