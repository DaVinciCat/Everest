using System;
using System.Collections.Generic;
using System.Linq;
using Everest.Http;
using Everest.Routing;
using Everest.Utils;
using Microsoft.OpenApi.Models;

namespace Everest.OpenApi
{
    public static class OpenApiDocumentExtensions
    {
        private static readonly Dictionary<string, OperationType> Map = new Dictionary<string, OperationType>()
        {
            { HttpMethods.Delete, OperationType.Delete },
            { HttpMethods.Get, OperationType.Get },
            { HttpMethods.Head, OperationType.Head },
            { HttpMethods.Options, OperationType.Options },
            { HttpMethods.Patch, OperationType.Patch },
            { HttpMethods.Post, OperationType.Post },
            { HttpMethods.Put, OperationType.Put }
        };

        public static string GetOpenApiPathItemKey(this RouteDescriptor descriptor)
        {
            return descriptor.Route.RoutePattern;
        }

        public static OperationType GetOpenApiOperationType(this RouteDescriptor descriptor)
        {
            var httpMethod = descriptor.Route.HttpMethod;

            if (!Map.TryGetValue(httpMethod, out var operation))
            {
                throw new ArgumentOutOfRangeException(nameof(httpMethod), $"Unsupported HTTP method: {httpMethod}");
            }

            return operation;
        }

        public static bool TryGetValue(this IEnumerable<OpenApiTag> tags, string key, out OpenApiTag tag)
        {
            tag = tags.FirstOrDefault(t => t.Name == key);
            return tag != null;
        }

        public static IEnumerable<T> GetAttributes<T>(this RouteDescriptor descriptor)
            where T : Attribute
        {
            return descriptor.EndPoint.Type.GetAttributes<T>().Union(descriptor.EndPoint.MethodInfo.GetAttributes<T>()).ToArray();
        }
    }
}
