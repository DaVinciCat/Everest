using System;

namespace Everest.OpenApi.Annotations
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class OpenApiIgnoreAttribute : Attribute
    {

    }
}
