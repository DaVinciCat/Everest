using System;

namespace Everest.OpenApi.Annotations
{
    public class QueryParameterAttribute : RequestParameterAttribute
    {
        public Type Type { get; }

        public bool IsOptional { get; set; }

        public bool AllowEmptyValue { get; set; }

        public bool Deprecated { get; set; }

        public QueryParameterAttribute(string name, Type type)
            : base(name)
        {
            Type = type;
        }
    }
}
