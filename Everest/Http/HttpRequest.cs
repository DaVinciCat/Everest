using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using Everest.Utils;

namespace Everest.Http
{
	public class HttpRequest
	{
		public Guid Id => request.RequestTraceIdentifier;

		public string HttpMethod => request.HttpMethod;

		public bool HasEntityBody => request.HasEntityBody;

		public string EntityBody => HasEntityBody ? entityBody ??= request.GetEntityBody() : null;

		public string EndPoint => request.Url?.AbsolutePath;

		public Stream InputStream => request.InputStream;

		public string AcceptEncoding => acceptEncoding ??= request.GetAcceptEncoding();

		public Encoding ContentEncoding => request.ContentEncoding;

		public NameValueCollection Headers => request.Headers;

		public NameValueCollection QueryString => request.QueryString;

		public QueryParameters QueryParameters { get; }
		
		private string entityBody;

		private string acceptEncoding;
		
		private readonly HttpListenerRequest request;

		public HttpRequest(HttpListenerRequest request)
		{
			this.request = request;
			QueryParameters = new QueryParameters(request.QueryString);
		}
	}
}
