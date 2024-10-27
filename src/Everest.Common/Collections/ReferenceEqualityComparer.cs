using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Everest.Common.Collections
{
    public sealed class ReferenceEqualityComparer : IEqualityComparer, IEqualityComparer<object>
    {
        private ReferenceEqualityComparer() { }

        public static IEqualityComparer Default { get; } = new ReferenceEqualityComparer();

        public new bool Equals(object x, object y) => x == y;

        public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
    }

    public sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T> where T : class
    {
        public static IEqualityComparer<T> Default { get; } = new ReferenceEqualityComparer<T>();

        public bool Equals(T x, T y) => ReferenceEquals(x, y);

        public int GetHashCode(T obj) => RuntimeHelpers.GetHashCode(obj);
    }
}
