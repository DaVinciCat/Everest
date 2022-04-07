using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Everest.Utils
{
	internal static class NameValueCollectionExtensions
	{
		internal static T GetValue<T>(this NameValueCollection collection, string key)
		{
			if (collection == null) 
				throw new ArgumentNullException(nameof(collection), "Collection is required.");

			if (key == null) 
				throw new ArgumentNullException(nameof(key), "Key is required.");

			if (collection[key] == null) 
				throw new ArgumentException($"Key is required: {key}.");

			var strValue = collection[key];
			var converter = TypeDescriptor.GetConverter(typeof(T));

			if (!converter.CanConvertFrom(typeof(string))) 
				throw new ArgumentException($"Failed to convert '{strValue}' to {typeof(T)}.");

			return (T)converter.ConvertFrom(strValue);
		}

		internal static bool TryGetValue<T>(this NameValueCollection collection, string key, out T value)
		{
			value = default(T);

			if (collection == null)
				throw new ArgumentNullException(nameof(collection), "Collection is required.");

			if (key == null)
				throw new ArgumentNullException(nameof(key), "Key is required.");

			if (collection[key] == null)
				return false;

			var strValue = collection[key];
			var converter = TypeDescriptor.GetConverter(typeof(T));

			if (!converter.CanConvertFrom(typeof(string)) || !converter.IsValid(strValue))
				return false;

			value = (T)converter.ConvertFrom(strValue);
			return true;
		}

		internal static T GetValue<T>(this NameValueCollection collection, string parameter, Func<string, T> parse)
		{
			try
			{
				var value = collection[parameter];
				if (value == null)
					throw new ArgumentException("Parameter is required.");

				return parse(value);
			}
			catch (Exception ex)
			{
				throw new Exception($"Failed to parse parameter '{parameter}': {ex.Message}", ex);
			}
		}
	}
}
