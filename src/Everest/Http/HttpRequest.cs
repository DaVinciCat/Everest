using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Everest.Collections;
using Everest.Utils;
using Microsoft.Extensions.Logging;

namespace Everest.Http
{
	public class HttpRequest
	{
		public ILogger<HttpRequest> Logger { get; }

		public Guid TraceIdentifier => request.RequestTraceIdentifier;

		public string HttpMethod => request.HttpMethod;

		public bool HasBody => request.HasEntityBody;

		public string Path => request.Url?.AbsolutePath.TrimEnd('/');

		public IPEndPoint RemoteEndPoint => request.RemoteEndPoint;

		public string Description => $"{HttpMethod} {Path}";

		public Stream InputStream => request.InputStream;
		
		public Encoding ContentEncoding => request.ContentEncoding;

		public NameValueCollection Headers => request.Headers;

		public ParameterCollection QueryParameters { get; set; }

		public ParameterCollection PathParameters { get; set; }

		private readonly HttpListenerRequest request;

		private readonly StreamPipe pipe;

		private readonly MemoryStream outputStream = new MemoryStream();

		public HttpRequest(HttpListenerRequest request, ILogger<HttpRequest> logger)
		{
            this.request = request ?? throw new ArgumentNullException(nameof(request));
            pipe = new StreamPipe(outputStream).PipeFrom(request.InputStream);

			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			QueryParameters = new ParameterCollection(request.QueryString);
			PathParameters = new ParameterCollection();
		}

		public bool HasHeader(string name) => request.Headers[name] != null;

		/*https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistenerrequest.inputstream?view=net-7.0*/
		public async Task<byte[]> ReadBodyAsync()
		{
			if (!request.HasEntityBody)
				return Array.Empty<byte>();

			if (outputStream.Length == 0)
				await pipe.FlushAsync();

			return outputStream.ToArray();
		}
	}

	public static class HttpRequestExtensions
	{
		public static async Task<string> ReadTextAsync(this HttpRequest request)
		{
			var data = await request.ReadBodyAsync();
			return request.ContentEncoding.GetString(data);
		}

		public static async Task<T> ReadJsonAsync<T>(this HttpRequest request, JsonSerializerOptions options = null)
		{
			var data = await request.ReadBodyAsync();

			return options == null ? 
				JsonSerializer.Deserialize<T>(data) : 
				JsonSerializer.Deserialize<T>(data, options);
		}

        public static async Task<NameValueCollection> ReadFormAsync(this HttpRequest request)
        {
            var data = await request.ReadBodyAsync();
            var content = request.ContentEncoding.GetString(data);
            return HttpUtility.ParseQueryString(content, request.ContentEncoding);
        }
	}
}
