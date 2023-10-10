using System;

namespace Everest.OpenApi.Annotations
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class RequestParameterAttribute : Attribute
    {
        public string Name { get; }

        public Type Type { get; }

        public bool IsOptional { get; set; }

        public string Description { get; set; }

        public bool Deprecated { get; set; }

        public bool AllowEmptyValue { get; set; }

        protected RequestParameterAttribute(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }

    public class QueryRequestParameterAttribute : RequestParameterAttribute
    {
        public QueryRequestParameterAttribute(string name, Type type) 
            : base(name, type)
        {

        }
    }
}
