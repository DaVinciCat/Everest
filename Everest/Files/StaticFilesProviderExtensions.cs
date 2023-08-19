using System;
using Everest.Http;
using System.IO;

namespace Everest.Files
{
    public static class StaticFilesProviderExtensions
    {
        public static bool IsStaticFileRequest(this IStaticFilesProvider provider, HttpRequest request)
        {
            if (provider == null) 
                throw new ArgumentNullException(nameof(provider));

            if (request == null) 
                throw new ArgumentNullException(nameof(request));

            return (request.IsGetMethod() || request.IsHeadMethod()) && !request.Path.EndsWith('/') && provider.TryGetFile(provider.GetFilePhysicalPath(request), out _);
        }

        public static string GetFilePhysicalPath(this IStaticFilesProvider provider, HttpRequest request)
        {
            return Path.Combine(provider.PhysicalPath, RequestPathToFilePath(request));
        }

        public static string RequestPathToFilePath(this HttpRequest request)
        {
           if (request == null)
                throw new ArgumentNullException(nameof(request));

           return request.Path.Trim('/').Replace("/", "\\");
        }
    }
}
