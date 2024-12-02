using System;
using System.IO;
using Everest.Http;

namespace Everest.StaticFiles
{
    public static class StaticFilesProviderExtensions
    {
        public static bool IsStaticFileRequest(this IStaticFilesProvider staticFilesProvider, IHttpRequest request)
        {
            if (staticFilesProvider == null) 
                throw new ArgumentNullException(nameof(staticFilesProvider));

            if (request == null) 
                throw new ArgumentNullException(nameof(request));

#if NET5_0_OR_GREATER
            return (request.IsGetMethod() || request.IsHeadMethod()) && !request.Path.EndsWith('/') && staticFilesProvider.TryGetFile(RequestPathToFilePath(request), out _);
#else
            return (request.IsGetMethod() || request.IsHeadMethod()) && !request.Path.EndsWith("/") && staticFilesProvider.TryGetFile(RequestPathToFilePath(request), out _);
#endif

        }

        public static string RequestPathToFilePath(this IHttpRequest request)
        {
           if (request == null)
                throw new ArgumentNullException(nameof(request));

           return request.Path.Trim('/').Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
