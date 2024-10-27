using System;
using Everest.Core.Http;

namespace Everest.Routing
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RestRouteAttribute : Attribute
    {
        public string HttpMethod { get; }

        public string RoutePattern { get; }

        public RestRouteAttribute(string httpMethod, string routePattern)
        {
            HttpMethod = httpMethod ?? throw new ArgumentNullException(nameof(httpMethod));
            RoutePattern = routePattern ?? throw new ArgumentNullException(nameof(routePattern));
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpGetAttribute : RestRouteAttribute
    {
        public HttpGetAttribute(string routePattern)
            : base(HttpMethods.Get, routePattern)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPostAttribute : RestRouteAttribute
    {
        public HttpPostAttribute(string routePattern)
            : base(HttpMethods.Post, routePattern)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPutAttribute : RestRouteAttribute
    {
        public HttpPutAttribute(string routePattern)
            : base(HttpMethods.Put, routePattern)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpDeleteAttribute : RestRouteAttribute
    {
        public HttpDeleteAttribute(string routePattern)
            : base(HttpMethods.Delete, routePattern)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpHeadAttribute : RestRouteAttribute
    {
        public HttpHeadAttribute(string routePattern)
            : base(HttpMethods.Head, routePattern)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPatchAttribute : RestRouteAttribute
    {
        public HttpPatchAttribute(string routePattern)
            : base(HttpMethods.Patch, routePattern)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpOptionsAttribute : RestRouteAttribute
    {
        public HttpOptionsAttribute(string routePattern)
            : base(HttpMethods.Options, routePattern)
        {

        }
    }
}
