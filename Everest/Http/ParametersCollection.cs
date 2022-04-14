using System;
using System.Collections.Specialized;
using Everest.Utils;

namespace Everest.Http
{
	public class ParametersCollection : NameValueCollection
	{
		public ParametersCollection()
		{

		}

		public ParametersCollection(NameValueCollection collection)
			: base(collection)
		{
			
		}
		
		public bool HasParams() => Count > 0;

		public bool HasParam(string parameter) => this[parameter] != null;

		public T GetParamValue<T>(string parameter)
		{
			return HasParam(parameter)
				? this.GetValue<T>(parameter)
				: throw new ArgumentException($"Parameter is required: {parameter}.");
		}

		public T GetParamValue<T>(string parameter, Func<string, T> parse)
		{
			return HasParam(parameter)
				? this.GetValue(parameter, parse)
				: throw new ArgumentException($"Parameter is required: {parameter}.");
		}

		public bool TryGetParamValue<T>(string parameter, out T value)
		{
			value = default;
			return HasParam(parameter) && this.TryGetValue(parameter, out value);
		}
	}
}
