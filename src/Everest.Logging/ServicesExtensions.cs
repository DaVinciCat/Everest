using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Everest.Logging
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddLoggerFactory(this IServiceCollection services, Func<IServiceProvider, ILoggerFactory> builder)
        {
            services.AddSingleton(builder);
            return services;
        }

        public static IServiceCollection AddConsoleLoggerFactory(this IServiceCollection services, Action<ILoggingBuilder> configurator = null)
        {
            var factory = LoggerFactory.Create(config =>
            {
                config.AddSimpleConsole(opt =>
                {
                    opt.SingleLine = true;
                    opt.ColorBehavior = LoggerColorBehavior.Enabled;
                    opt.IncludeScopes = false;
                    opt.TimestampFormat = "hh:mm:ss:ffff ";
                });

                config.SetMinimumLevel(LogLevel.Trace);
                configurator?.Invoke(config);
            });

            return services.AddSingleton(factory);
        }
    }
}
