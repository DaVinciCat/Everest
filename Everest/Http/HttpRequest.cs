using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Everest.Utils;

namespace Everest.Http
{
	public class HttpRequest
	{
		public Guid Id => request.RequestTraceIdentifier;

		public string HttpMethod => request.HttpMethod;

		public bool HasPayload => request.HasEntityBody;

		public string Payload => HasPayload ? payload ??= request.ReadPayload() : null;

		public string EndPoint => request.Url?.AbsolutePath;

		public Stream InputStream => request.InputStream;

		public string AcceptEncoding => acceptEncoding ??= request.GetAcceptEncoding();

		public Encoding ContentEncoding => request.ContentEncoding;

		public NameValueCollection Headers => request.Headers;
		
		public NameValueCollection Parameters => parameters ??= request.Url?.Query != null
			? HttpUtility.ParseQueryString(request.Url.Query, Encoding.UTF8)
			: new NameValueCollection();

		public bool HasParameter(string parameter) => Parameters[parameter] != null;

		public T GetParameterValue<T>(string parameter) => Parameters.GetParameterValue<T>(parameter);
		
		public T GetParameterValue<T>(string parameter, Func<string, T> parse) => Parameters.GetParameterValue<T>(parameter, parse);

		public bool TryGetParameterValue<T>(string parameter, out T value) => Parameters.TryGetParameterValue(parameter, out value);
		
		private string payload;

		private string acceptEncoding;

		private NameValueCollection parameters;
		
		private readonly HttpListenerRequest request;

		public HttpRequest(HttpListenerRequest request)
		{
			this.request = request;
		}
	}
}
