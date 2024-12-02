using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Everest.Mime;
using Everest.Utils;
using Microsoft.Extensions.Logging;

namespace Everest.StaticFiles
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddStaticFileRequestHandler(this IServiceCollection services, Func<IServiceProvider, IStaticFileRequestHandler> builder)
        {
            services.AddSingleton(builder);
            return services;
        }

        public static IServiceCollection AddStaticFileRequestHandler(this IServiceCollection services)
        {
            services.AddSingleton<IStaticFileRequestHandler>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var staticFilesProvider = provider.GetRequiredService<IStaticFilesProvider>();
                var mimeTypesProvider = provider.GetRequiredService<IMimeTypesProvider>();
                return new StaticFileRequestHandler(staticFilesProvider, mimeTypesProvider, loggerFactory.CreateLogger<StaticFileRequestHandler>());
            });

            return services;
        }

        public static IServiceCollection AddStaticFileRequestHandler(this IServiceCollection services, Action<StaticFileRequestHandlerConfigurator> configurator)
        {
            services.AddSingleton<IStaticFileRequestHandler>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var staticFilesProvider = provider.GetRequiredService<IStaticFilesProvider>();
                var mimeTypesProvider = provider.GetRequiredService<IMimeTypesProvider>();
                var staticFileRequestHandler = new StaticFileRequestHandler(staticFilesProvider, mimeTypesProvider, loggerFactory.CreateLogger<StaticFileRequestHandler>());
                configurator(new StaticFileRequestHandlerConfigurator(staticFileRequestHandler, provider));

                return staticFileRequestHandler;
            });

            return services;
        }

        public static IServiceCollection AddStaticFilesProvider(this IServiceCollection services, Func<IServiceProvider, IStaticFilesProvider> builder)
        {
            services.AddSingleton(builder);
            return services;
        }

        public static IServiceCollection AddStaticFilesProvider(this IServiceCollection services)
        {
            services.AddSingleton<IStaticFilesProvider>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

                const string path = "public";
                var di = new DirectoryInfo(path);
                di.CreateDirectory();

                return new StaticFilesProvider(path, loggerFactory.CreateLogger<StaticFilesProvider>());
            });

            return services;
        }

        public static IServiceCollection AddStaticFilesProvider(this IServiceCollection services, Action<StaticFilesProviderConfigurator> configurator)
        {
            services.AddSingleton<IStaticFilesProvider>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

                const string path = "public";
                var di = new DirectoryInfo(path);
                di.CreateDirectory();

                var staticFilesProvider = new StaticFilesProvider(path, loggerFactory.CreateLogger<StaticFilesProvider>());
                configurator(new StaticFilesProviderConfigurator(staticFilesProvider, provider));

                return staticFilesProvider;
            });

            return services;
        }
    }
}
