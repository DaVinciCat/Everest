using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace Everest.Collections
{
	public class PrefixCollection : ICollection<string>
	{
		public int Count => collection.Count;

		public bool IsReadOnly => collection.IsReadOnly;

		public bool IsSynchronized => collection.IsSynchronized;

		private readonly HttpListenerPrefixCollection collection;

		public PrefixCollection(HttpListenerPrefixCollection collection)
		{
			this.collection = collection;
		}

		public void Add(string item) => collection.Add(item);

		public void Clear() => collection.Clear();

		public bool Contains(string item) => collection.Contains(item);

		public void CopyTo(Array array, int arrayIndex) => collection.CopyTo(array, arrayIndex);

		public void CopyTo(string[] array, int arrayIndex) => collection.CopyTo(array, arrayIndex);

		public IEnumerator<string> GetEnumerator() => collection.GetEnumerator();

		public bool Remove(string item) => collection.Remove(item);

		IEnumerator IEnumerable.GetEnumerator() => collection.GetEnumerator();
	}
}
