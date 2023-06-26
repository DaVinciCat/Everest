using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using Everest.Collections;

namespace Everest.Http
{
	public class HttpRequest
	{
		public string HttpMethod => request.HttpMethod;

		public bool HasPayload => request.HasEntityBody;

		public string EndPoint => request.Url?.AbsolutePath;

		public IPEndPoint RemoteEndPoint => request.RemoteEndPoint;

		public string Description => $"{HttpMethod} {EndPoint}";

		public Stream InputStream => request.InputStream;
		
		public Encoding ContentEncoding => request.ContentEncoding;

		public NameValueCollection Headers => request.Headers;

		public ParameterCollection QueryParameters { get; set; }

		public ParameterCollection PathParameters { get; set; }

		public string Payload => payload ??= ReadPayload();

		private string payload;

		private readonly HttpListenerRequest request;

		public HttpRequest(HttpListenerRequest request)
		{
			this.request = request ?? throw new ArgumentNullException(nameof(request));
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
}
