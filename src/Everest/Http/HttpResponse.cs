using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Everest.Utils;
using Microsoft.Extensions.Logging;

namespace Everest.Http
{
	public class HttpResponse
	{
		public ILogger<HttpResponse> Logger { get; }

		public Guid TraceIdentifier => request.RequestTraceIdentifier;

		public bool ResponseSent { get; private set; }

		public bool ResponseClosed { get; private set; }

		public bool SendChunked
		{
			get => response.SendChunked;
			set => response.SendChunked = value;
		}

		///*https://learn.microsoft.com/ru-ru/dotnet/api/system.net.httplistenerresponse.contentlength64?view=net-7.0*/
		//public long ContentLength64
		//{
		//	get => response.ContentLength64;
		//	set => response.ContentLength64 = value;
		//}

		public long ContentLength => InputStream.Length;

		public bool KeepAlive
		{
			get => response.KeepAlive;
			set => response.KeepAlive = value;
		}

		public string ContentType
		{
			get => response.ContentType;
			set => response.ContentType = value;
		}

		public string ContentDisposition
		{
			get => Headers[HttpHeaders.ContentDisposition];
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					Headers.Remove(HttpHeaders.ContentDisposition);
				}
				else
				{
					Headers.Set(HttpHeaders.ContentDisposition, value);
				}
			}
		}

		public Encoding ContentEncoding
		{
			get => response.ContentEncoding ?? Encoding.UTF8;
			set => response.ContentEncoding = value;
		}

		public NameValueCollection Headers => response.Headers;

		public HttpStatusCode StatusCode
		{
			get => (HttpStatusCode)response.StatusCode;
			set
			{
				response.StatusCode = (int)value;
				response.StatusDescription = value.ToString();
			}
		}

		public Stream InputStream
		{
			get => inputStream;
			set
			{
				if (value == null)
					throw new ArgumentNullException("Input stream cannot be null");

				if (!value.CanRead)
				{
					throw new InvalidOperationException("Input stream is not readable");
				}

				inputStream = value;
			}
		}

		public Stream OutputStream
		{
			get => outputStream;
			set
			{
				if (value == null)
					throw new ArgumentNullException("Output stream cannot be null");

				if (!value.CanWrite)
				{
					throw new InvalidOperationException("Output stream is not writable");
				}

				outputStream = value;
			}
		}

		private Stream inputStream;

		private Stream outputStream;

		private readonly HttpListenerResponse response;

		private readonly HttpListenerRequest request;

		public HttpResponse(HttpListenerResponse response, HttpListenerRequest request, ILogger<HttpResponse> logger)
		{
			this.response = response ?? throw new ArgumentNullException(nameof(response));
			this.request = request ?? throw new ArgumentNullException(nameof(request));

			InputStream = new MemoryStream();
			OutputStream = response.OutputStream;
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			StatusCode = HttpStatusCode.OK;
			ContentEncoding = Encoding.UTF8;

			AppendHeader(HttpHeaders.Server, "Everest");
		}

		public bool HasHeader(string name) => response.Headers[name] != null;

		public void AddHeader(string name, string value) => response.AddHeader(name, value);

		public void AppendHeader(string name, string value) => response.AppendHeader(name, value);

		public void RemoveHeader(string name) => response.Headers.Remove(name);

		public async Task RedirectAsync(string url)
		{
			response.Redirect(url);
			await Task.CompletedTask;
		}

		public async Task SendAsync()
		{
			try
			{
				try
				{
					Logger.LogTrace($"{TraceIdentifier} - Sending response: {new { RemoteEndPoint = request.RemoteEndPoint, ContentLength = ContentLength.ToReadableSize(), StatusCode = response.StatusCode, ContentType = response.ContentType, ContentEncoding = response.ContentEncoding?.EncodingName }}");

					if (InputStream.CanSeek)
					{
						InputStream.Position = 0;
					}

					var buffer = new byte[4096];
					int read;

					while ((read = await InputStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
					{
						await OutputStream.WriteAsync(buffer, 0, read);
					}
				}
				catch
				{
					StatusCode = HttpStatusCode.InternalServerError;
					throw;
				}
				finally
				{

#if NET5_0_OR_GREATER
					await InputStream.DisposeAsync();
					await OutputStream.DisposeAsync();
#else
            		InputStream.Dispose();
            		OutputStream.Dispose();

           			await Task.CompletedTask;
#endif
					ResponseSent = true;
					Logger.LogTrace($"{TraceIdentifier} - Response sent");
				}
			}
			finally
			{
				Logger.LogTrace($"{TraceIdentifier} - Closing response");
				response.Close();
				ResponseClosed = true;
				Logger.LogTrace($"{TraceIdentifier} - Response closed");
			}
		}
	}

	public static class HttpResponseExtensions
	{
		public static async Task WriteAsync(this HttpResponse response, byte[] content)
		{
			if (content == null)
				throw new ArgumentNullException(nameof(content));

			await response.InputStream.WriteAsync(content, 0, content.Length);
		}

		public static async Task WriteAsync(this HttpResponse response, string content)
		{
			if (response == null)
				throw new ArgumentNullException(nameof(response));

			if (content == null)
				throw new ArgumentNullException(nameof(content));

			var bytes = response.ContentEncoding.GetBytes(content);
			await response.WriteAsync(bytes);
		}

		public static async Task WriteTextAsync(this HttpResponse response, string content)
		{
			if (response == null)
				throw new ArgumentNullException(nameof(response));

			if (content == null)
				throw new ArgumentNullException(nameof(content));

			response.ContentType = "text/plain";
			await response.WriteAsync(content);
		}

		public static async Task WriteHtmlAsync(this HttpResponse response, string content)
		{
			if (response == null)
				throw new ArgumentNullException(nameof(response));

			if (content == null)
				throw new ArgumentNullException(nameof(content));

			response.ContentType = "text/html";
			await response.WriteAsync(content);
		}

		public static async Task WriteJsonAsync<T>(this HttpResponse response, T content, JsonSerializerOptions options = null)
		{
			if (response == null)
				throw new ArgumentNullException(nameof(response));

			if (content == null)
				throw new ArgumentNullException(nameof(content));

			var json = options == null ?
				JsonSerializer.Serialize(content) :
				JsonSerializer.Serialize(content, options);

			response.ContentType = "application/json";
			await response.WriteAsync(json);
		}

		public static Task WriteFileAsync(this HttpResponse response, string filename, ContentType contentType, ContentDisposition contentDisposition)
		{
			if (response == null)
				throw new ArgumentNullException(nameof(response));

			if (filename == null)
				throw new ArgumentNullException(nameof(filename));

			if (contentType == null)
				throw new ArgumentNullException(nameof(contentType));

			if (contentDisposition == null)
				throw new ArgumentNullException(nameof(contentDisposition));

			var file = new FileInfo(filename);
			var fs = file.OpenRead();
			response.ContentType = contentType.MediaType;
			response.ContentDisposition = contentDisposition.DispositionType;
			response.InputStream = fs;

			return Task.CompletedTask;
		}

		public static Task WriteFileAsync(this HttpResponse response, string filename, string contentType, string contentDisposition)
		{
			if (response == null)
				throw new ArgumentNullException(nameof(response));

			if (filename == null)
				throw new ArgumentNullException(nameof(filename));

			if (contentType == null)
				throw new ArgumentNullException(nameof(contentType));

			if (contentDisposition == null)
				throw new ArgumentNullException(nameof(contentDisposition));

			var file = new FileInfo(filename);
			var fs = file.OpenRead();
			response.ContentType = contentType;
			response.ContentDisposition = contentDisposition;
			response.InputStream = fs;

			return Task.CompletedTask;
		}
	}
}
