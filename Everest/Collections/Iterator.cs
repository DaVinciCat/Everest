﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Everest.Collections
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
			return position < Items.Length;
		}

		public bool HasNext()
		{
			return position < Items.Length - 1;
		}

		public void Reset()
		{
			position = -1;
		}

		public void Dispose()
		{

		}
	}

	public static class IteratorExtensions
	{
		public static Iterator<T> ToIterator<T>(this IEnumerable<T> enumerable)
		{
			return new Iterator<T>(enumerable.ToArray());
		}
	}
}
