using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Everest.Utils
{
    public static class ReflectionExtensions
    {
        public static IEnumerable<T> GetAttributes<T>(this MethodInfo method)
            where T : Attribute
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            foreach (var attribute in method.GetCustomAttributes(typeof(T), false).OfType<T>())
            {
                yield return attribute;
            }
        }

        public static IEnumerable<T> GetAttributes<T>(this Type type)
            where T : Attribute
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            foreach (var attribute in type.GetCustomAttributes(typeof(T), false).OfType<T>())
            {
                yield return attribute;
            }
        }
    }
}
