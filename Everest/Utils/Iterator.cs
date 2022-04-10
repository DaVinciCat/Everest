using System;
using System.Collections;
using System.Collections.Generic;

namespace Everest.Utils
{
	public class Iterator<T> : IEnumerator<T>
	{
		public T Current
		{
			get
			{
				try
				{
					return Items[position];
				}
				catch (IndexOutOfRangeException)
				{
					throw new InvalidOperationException();
				}
			}
		}

		object IEnumerator.Current => Current;

		public T[] Items { get; }

		private int position = -1;

		public Iterator(T[] items)
		{
			Items = items;
		}

		public bool MoveNext()
		{
			position++;
			return (position < Items.Length);
		}

		public bool HasNext()
		{
			return (position + 1 < Items.Length);
		}

		public void Reset()
		{
			position = -1;
		}

		public void Dispose()
		{

		}
	}
}
