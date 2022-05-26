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

		public string EndPoint => request.Url?.AbsolutePath;

		public string Description => $"{HttpMethod} {EndPoint}";

		public Stream InputStream => request.InputStream;
		
		public Encoding ContentEncoding => request.ContentEncoding;

		public NameValueCollection Headers => request.Headers;

		public ParametersCollection QueryParameters { get; }

		public ParametersCollection PathParameters { get; internal set; }

		private readonly HttpListenerRequest request;

		public HttpRequest(HttpListenerRequest request)
		{
			this.request = request;
			QueryParameters = new ParametersCollection(request.QueryString);
			PathParameters = new ParametersCollection();
		}

		public string ReadEntityBody()
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
