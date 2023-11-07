using System;

namespace Everest.OpenApi.Annotations
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OperationAttribute : Attribute
    {
        public string OperationId { get; set; }

        public string Description { get; set; }

        public string Summary { get; set; }

        public bool Deprecated { get; set; }
    }
}
