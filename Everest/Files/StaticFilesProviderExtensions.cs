using System;
using Everest.Http;

namespace Everest.Files
{
    public static class StaticFilesProviderExtensions
    {
        public static bool IsStaticFileRequest(this IStaticFilesProvider staticFilesProvider, HttpRequest request)
        {
            if (staticFilesProvider == null) 
                throw new ArgumentNullException(nameof(staticFilesProvider));

            if (request == null) 
                throw new ArgumentNullException(nameof(request));

            return (request.IsGetMethod() || request.IsHeadMethod()) && !request.Path.EndsWith('/') && staticFilesProvider.TryGetFile(RequestPathToFilePath(request), out _);
        }
        
        public static string RequestPathToFilePath(this HttpRequest request)
        {
           if (request == null)
                throw new ArgumentNullException(nameof(request));

           return request.Path.Trim('/').Replace("/", "\\");
        }
    }
}
