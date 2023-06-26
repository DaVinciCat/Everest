using System;

namespace Everest.Rest
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
            : base("GET", routePattern)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPostAttribute : RestRouteAttribute
    {
        public HttpPostAttribute(string routePattern)
            : base("POST", routePattern)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPutAttribute : RestRouteAttribute
    {
        public HttpPutAttribute(string routePattern)
            : base("PUT", routePattern)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpDeleteAttribute : RestRouteAttribute
    {
        public HttpDeleteAttribute(string routePattern)
            : base("DELETE", routePattern)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpHeadAttribute : RestRouteAttribute
    {
        public HttpHeadAttribute(string routePattern)
            : base("HEAD", routePattern)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPatchAttribute : RestRouteAttribute
    {
        public HttpPatchAttribute(string routePattern)
            : base("PATCH", routePattern)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpOptionsAttribute : RestRouteAttribute
    {
        public HttpOptionsAttribute(string routePattern)
            : base("OPTIONS", routePattern)
        {

        }
    }
}
