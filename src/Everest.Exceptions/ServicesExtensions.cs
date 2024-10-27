using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Logging;

namespace Everest.Exceptions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddExceptionHandler(this IServiceCollection services, Func<IServiceProvider, IExceptionHandler> builder)
        {
            services.AddSingleton(builder);
            return services;
        }

        public static IServiceCollection AddExceptionHandler(this IServiceCollection services)
        {
            services.AddSingleton<IExceptionHandler>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                return new ExceptionHandler(loggerFactory.CreateLogger<ExceptionHandler>());
            });

            return services;
        }

        public static IServiceCollection AddExceptionHandler(this IServiceCollection services, Action<ExceptionHandlerConfigurator> configurator)
        {
            services.AddSingleton<IExceptionHandler>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var exceptionHandler = new ExceptionHandler(loggerFactory.CreateLogger<ExceptionHandler>());
                configurator(new ExceptionHandlerConfigurator(exceptionHandler, provider));

                return exceptionHandler;
            });

            return services;
        }
    }
}
