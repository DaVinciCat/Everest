using System;

namespace Everest.OpenApi.Annotations
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class TagsAttribute : Attribute
    {
        public string[] Tags { get; }

        public TagsAttribute(params string[] tags)
        {
            Tags = tags;
        }
    }
}
