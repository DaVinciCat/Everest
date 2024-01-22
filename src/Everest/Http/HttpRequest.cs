using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Everest.Collections;
using Microsoft.Extensions.Logging;

namespace Everest.Http
{
	public class HttpRequest : IHttpRequest
    {
        ILogger<IHttpRequest> IHttpRequest.Logger => Logger;

        public ILogger<HttpRequest> Logger { get; }
		
        public Guid TraceIdentifier => request.RequestTraceIdentifier;

		public string HttpMethod => request.HttpMethod;

		public bool HasRequestBody => request.HasEntityBody;

		public string Path => request.Url?.AbsolutePath.TrimEnd('/');

		public IPEndPoint RemoteEndPoint => request.RemoteEndPoint;

		public string Description => $"{HttpMethod} {Path}";

		public Encoding ContentEncoding => request.ContentEncoding;

		public NameValueCollection Headers => request.Headers;

		public ParameterCollection QueryParameters { get; set; }

		public ParameterCollection PathParameters { get; set; }

		public Stream InputStream => request.InputStream;
		
        private readonly HttpListenerRequest request;

		public HttpRequest(HttpListenerRequest request, ILogger<HttpRequest> logger)
		{
			this.request = request ?? throw new ArgumentNullException(nameof(request));

			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			QueryParameters = new ParameterCollection(request.QueryString);
			PathParameters = new ParameterCollection();
		}

		public bool HasHeader(string name) => request.Headers[name] != null;

		/*https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistenerrequest.inputstream?view=net-7.0*/
		public virtual async Task<byte[]> ReadRequestBodyAsync()
		{
			if (!HasRequestBody)
				return Array.Empty<byte>();

			if (InputStream.CanSeek)
			{
				InputStream.Position = 0;
			}

			var buffer = new byte[4096];
			using (var ms = new MemoryStream())
			{
				try
				{
					int read;
					while ((read = await InputStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
					{
						ms.Write(buffer, 0, read);
					}

					return ms.ToArray();
				}
				finally
				{
					ms.Close();			
					InputStream.Close();
                }
			}
		}
	}

	public static class HttpRequestExtensions
	{
		public static async Task<string> ReadRequestBodyAsTextAsync(this IHttpRequest request)
		{
			var data = await request.ReadRequestBodyAsync();
			return request.ContentEncoding.GetString(data);
		}

		public static async Task<T> ReadRequestBodyAsJsonAsync<T>(this IHttpRequest request, JsonSerializerOptions options = null)
		{
			var data = await request.ReadRequestBodyAsync();

			return options == null ?
				JsonSerializer.Deserialize<T>(data) :
				JsonSerializer.Deserialize<T>(data, options);
		}

		public static async Task<NameValueCollection> ReadRequestBodyAsFormAsync(this IHttpRequest request)
		{
			var data = await request.ReadRequestBodyAsync();
			var content = request.ContentEncoding.GetString(data);
			return HttpUtility.ParseQueryString(content, request.ContentEncoding);
		}
	}
}
