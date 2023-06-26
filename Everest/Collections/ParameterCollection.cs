using System;
using System.Collections.Specialized;

namespace Everest.Collections
{
	public class ParameterCollection : NameValueCollection
	{
		public ParameterCollection()
		{

		}

		public ParameterCollection(NameValueCollection collection)
			: base(collection)
		{
			
		}
		
		public bool HasParameters() => Count > 0;

		public bool HasParameter(string parameter) => this[parameter] != null;

		public T GetParameterValue<T>(string parameter)
		{
			return HasParameter(parameter)
				? this.GetValue<T>(parameter)
				: throw new ArgumentException($"Parameter required: {parameter}.");
		}

		public T GetParameterValue<T>(string parameter, Func<string, T> parse)
		{
			return HasParameter(parameter)
				? this.GetValue(parameter, parse)
				: throw new ArgumentException($"Parameter required: {parameter}.");
		}

		public bool TryGetParameterValue<T>(string parameter, out T value)
		{
			value = default;
			return HasParameter(parameter) && this.TryGetValue(parameter, out value);
		}
	}
}
