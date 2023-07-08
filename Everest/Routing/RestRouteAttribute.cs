using System;

namespace Everest.Routing
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RestRouteAttribute : Attribute
    {
        public string HttpMethod { get; }

        public string RoutePath { get; }

        public RestRouteAttribute(string httpMethod, string routePath)
        {
            HttpMethod = httpMethod ?? throw new ArgumentNullException(nameof(httpMethod));
            RoutePath = routePath ?? throw new ArgumentNullException(nameof(routePath));
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpGetAttribute : RestRouteAttribute
    {
        public HttpGetAttribute(string routePath)
            : base("GET", routePath)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPostAttribute : RestRouteAttribute
    {
        public HttpPostAttribute(string routePath)
            : base("POST", routePath)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPutAttribute : RestRouteAttribute
    {
        public HttpPutAttribute(string routePath)
            : base("PUT", routePath)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpDeleteAttribute : RestRouteAttribute
    {
        public HttpDeleteAttribute(string routePath)
            : base("DELETE", routePath)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpHeadAttribute : RestRouteAttribute
    {
        public HttpHeadAttribute(string routePath)
            : base("HEAD", routePath)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPatchAttribute : RestRouteAttribute
    {
        public HttpPatchAttribute(string routePath)
            : base("PATCH", routePath)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpOptionsAttribute : RestRouteAttribute
    {
        public HttpOptionsAttribute(string routePath)
            : base("OPTIONS", routePath)
        {

        }
    }
}
