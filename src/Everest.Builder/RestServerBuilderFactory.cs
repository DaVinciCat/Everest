using System.IO;
using Everest.Common.Io;
using Everest.Compression;
using Everest.Core.Rest;
using Everest.EndPoints;
using Everest.Exceptions;
using Everest.Mime;
using Everest.Routing;
using Everest.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Everest.Builder
{
    public static class RestServerBuilderFactory
    {
        public static RestServerBuilder CreateBuilder(IServiceCollection services = null)
        {
            if (services == null)
            {
                services = new ServiceCollection();
            }

            services.TryAddSingleton<ILoggerFactory>(new NullLoggerFactory());

            services.TryAddSingleton<IExceptionHandler>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                return new ExceptionHandler(loggerFactory.CreateLogger<ExceptionHandler>());
            });

            services.TryAddSingleton<IRouteScanner>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                return new RouteScanner(loggerFactory.CreateLogger<RouteScanner>());
            });

            services.TryAddSingleton<IRouter>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                return new Router(loggerFactory.CreateLogger<Router>());
            });

            services.TryAddSingleton<IStaticFilesProvider>(provider =>
                     {
                         var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

                         const string path = "public";
                         var di = new DirectoryInfo(path);
                         di.CreateDirectory();

                         return new StaticFilesProvider(path, loggerFactory.CreateLogger<StaticFilesProvider>());
                     });

            services.TryAddSingleton<IMimeTypesProvider>(_ => new MimeTypesProvider());

            services.TryAddSingleton<IStaticFileRequestHandler>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var staticFilesProvider = provider.GetRequiredService<IStaticFilesProvider>();
                var mimeTypesProvider = provider.GetRequiredService<IMimeTypesProvider>();
                return new StaticFileRequestHandler(staticFilesProvider, mimeTypesProvider, loggerFactory.CreateLogger<StaticFileRequestHandler>());
            });
            
            services.TryAddSingleton<IResponseCompressor>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                return new ResponseCompressor(loggerFactory.CreateLogger<ResponseCompressor>());
            });

            services.TryAddSingleton<IEndPointInvoker>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                return new EndPointInvoker(loggerFactory.CreateLogger<EndPointInvoker>());
            });
            
            return new RestServerBuilder(services);
        }
    }
}
