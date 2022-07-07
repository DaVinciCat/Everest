using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace Everest.Http
{
	public class HttpRequest
	{
		public string HttpMethod => request.HttpMethod;

		public bool HasPayload => request.HasEntityBody;

		public string EndPoint => request.Url?.AbsolutePath;

		public string Description => $"{HttpMethod} {EndPoint}";

		public Stream InputStream => request.InputStream;
		
		public Encoding ContentEncoding => request.ContentEncoding;

		public NameValueCollection Headers => request.Headers;

		public ParameterCollection QueryParameters { get; }

		public ParameterCollection PathParameters { get; internal set; }

		public string Payload => payload ??= ReadPayload();

		private string payload;

		private readonly HttpListenerRequest request;

		public HttpRequest(HttpListenerRequest request)
		{
			this.request = request;
			QueryParameters = new ParameterCollection(request.QueryString);
			PathParameters = new ParameterCollection();
		}

		private string ReadPayload()
		{
			if (!request.HasEntityBody)
				return null;

			using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
			{
				return reader.ReadToEnd();
			}
		}
	}

	public static class HttpRequestExtensions
	{
		public static bool IsCorsPreflight(this HttpRequest request)
		{
			return request.HttpMethod == "OPTIONS" &&
			       request.Headers["Access-Control-Request-Method"] != null &&
			       request.Headers["Access-Control-Request-Headers"] != null &&
			       request.Headers["Origin"] != null;
		}
	}
}
