using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace Everest.Http
{
	public class HttpRequest
	{
		public Guid Id => request.RequestTraceIdentifier;

		public string HttpMethod => request.HttpMethod;

		public bool HasEntityBody => request.HasEntityBody;

		public string EntityBody => entityBody ??= GetEntityBody();

		public string EndPoint => request.Url?.AbsolutePath;

		public Stream InputStream => request.InputStream;
		
		public Encoding ContentEncoding => request.ContentEncoding;

		public NameValueCollection Headers => request.Headers;

		public NameValueCollection QueryString => request.QueryString;

		public QueryParameters QueryParameters { get; }

		private string entityBody;

		private readonly HttpListenerRequest request;

		public HttpRequest(HttpListenerRequest request)
		{
			this.request = request;
			QueryParameters = new QueryParameters(request.QueryString);
		}

		private string GetEntityBody()
		{
			if (!request.HasEntityBody)
				return null;

			using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
			{
				return reader.ReadToEnd();
			}
		}
	}
}
