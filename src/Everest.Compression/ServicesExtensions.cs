using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Logging;

namespace Everest.Compression
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddResponseCompressor(this IServiceCollection services, Func<IServiceProvider, IResponseCompressor> builder)
        {
            services.AddSingleton(builder);
            return services;
        }

        public static IServiceCollection AddResponseCompressor(this IServiceCollection services)
        {
            services.AddSingleton<IResponseCompressor>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                return  new ResponseCompressor(loggerFactory.CreateLogger<ResponseCompressor>());
            });

            return services;
        }

        public static IServiceCollection AddResponseCompressor(this IServiceCollection services, Action<ResponseCompressorConfigurator> configurator)
        {
            services.AddSingleton<IResponseCompressor>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var responseCompressor = new ResponseCompressor(loggerFactory.CreateLogger<ResponseCompressor>());
                configurator(new ResponseCompressorConfigurator(responseCompressor, provider));

                return responseCompressor;
            });

            return services;
        }
    }
}
