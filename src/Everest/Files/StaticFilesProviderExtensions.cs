﻿using System;
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

#if NET5_0_OR_GREATER
            return (request.IsGetMethod() || request.IsHeadMethod()) && !request.Path.EndsWith('/') && staticFilesProvider.TryGetFile(RequestPathToFilePath(request), out _);
#else
            return (request.IsGetMethod() || request.IsHeadMethod()) && !request.Path.EndsWith("/") && staticFilesProvider.TryGetFile(RequestPathToFilePath(request), out _);
#endif

        }

        public static string RequestPathToFilePath(this HttpRequest request)
        {
           if (request == null)
                throw new ArgumentNullException(nameof(request));

           return request.Path.Trim('/').Replace("/", "\\");
        }
    }
}