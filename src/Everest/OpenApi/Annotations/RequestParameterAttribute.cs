using System;

namespace Everest.OpenApi.Annotations
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class RequestParameterAttribute : Attribute
    {
        public string Name { get; }
        
        public string Description { get; set; }

        public bool Deprecated { get; set; }

        protected RequestParameterAttribute(string name)
        {
            Name = name;
        }
    }
}
