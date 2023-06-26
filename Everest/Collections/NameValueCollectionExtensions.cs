using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Everest.Collections
{
	public static class NameValueCollectionExtensions
	{
		public static T GetValue<T>(this NameValueCollection collection, string key)
		{
			if (collection == null)
				throw new ArgumentNullException(nameof(collection), "Collection required.");

			if (key == null)
				throw new ArgumentNullException(nameof(key), "Key required.");

			if (collection[key] == null)
				throw new KeyNotFoundException($"Key required: {key}.");

			var strValue = collection[key];
			var converter = TypeDescriptor.GetConverter(typeof(T));

			if (!converter.CanConvertFrom(typeof(string)))
				throw new InvalidOperationException($"Cannot convert {strValue} to {typeof(T)}.");

			return (T)converter.ConvertFrom(strValue);
		}

		public static T GetValue<T>(this NameValueCollection collection, string key, Func<string, T> parse)
		{
			if (collection == null)
				throw new ArgumentNullException(nameof(collection), "Collection required.");

			if (key == null)
				throw new ArgumentNullException(nameof(key), "Key required.");

			if (collection[key] == null)
				throw new KeyNotFoundException($"Key required: {key}.");

			var strValue = collection[key];

			try
			{
				return parse(strValue);
			}
			catch (Exception ex)
			{
				throw new Exception($"Failed to parse value: {strValue}: {ex.Message}", ex);
			}
		}

		public static bool TryGetValue<T>(this NameValueCollection collection, string key, out T value)
		{
			value = default(T);

			if (collection == null)
				throw new ArgumentNullException(nameof(collection), "Collection required.");

			if (key == null)
				throw new ArgumentNullException(nameof(key), "Key required.");

			if (collection[key] == null)
				return false;

			var strValue = collection[key];
			var converter = TypeDescriptor.GetConverter(typeof(T));

			if (!converter.CanConvertFrom(typeof(string)) || !converter.IsValid(strValue))
				return false;

			value = (T)converter.ConvertFrom(strValue);
			return true;
		}
	}
}
