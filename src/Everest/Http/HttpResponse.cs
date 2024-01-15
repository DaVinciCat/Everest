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

		public bool SendChunked
		{
			get => response.SendChunked;
			set => response.SendChunked = value;
		}

		///*https://learn.microsoft.com/ru-ru/dotnet/api/system.net.httplistenerresponse.contentlength64?view=net-7.0*/
		public long ContentLength64
		{
			get => response.ContentLength64;
			set => response.ContentLength64 = value;
		}

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

		public HttpResponseWriter ResponseWriter { get; set; }

        public Stream OutputStream => response.OutputStream;
		
		private readonly HttpListenerResponse response;

		private readonly HttpListenerRequest request;

		public HttpResponse(HttpListenerResponse response, HttpListenerRequest request, ILogger<HttpResponse> logger)
		{
			this.response = response ?? throw new ArgumentNullException(nameof(response));
			this.request = request ?? throw new ArgumentNullException(nameof(request));

			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			StatusCode = HttpStatusCode.OK;
			ContentEncoding = Encoding.UTF8;
            ResponseWriter = new HttpResponseWriter(this);
			
            AppendHeader(HttpHeaders.Server, "Everest");
		}

		public void SetCookie(Cookie cookie) => response.SetCookie(cookie);

		public bool HasHeader(string name) => response.Headers[name] != null;

		public void AddHeader(string name, string value) => response.AddHeader(name, value);

		public void AppendHeader(string name, string value) => response.AppendHeader(name, value);

		public void RemoveHeader(string name) => response.Headers.Remove(name);

		public void CloseResponse() => response.Close();

		public async Task RedirectAsync(string url)
		{
			response.Redirect(url);
			await Task.CompletedTask;
		}
		
		public Task SendEmptyResponseAsync()
		{
			Logger.LogTrace($"{TraceIdentifier} - Sending response: {new { RemoteEndPoint = request.RemoteEndPoint, ContentLength = ContentLength64.ToReadableSize(), StatusCode = response.StatusCode, ContentType = response.ContentType, ContentEncoding = response.ContentEncoding?.EncodingName }}");

			if (!OutputStream.CanWrite)
			{
				throw new InvalidOperationException("Output stream is not writable");
			}

			try
			{
				OutputStream.Close();
			}
			finally
			{
				ResponseSent = true;
				response.Close();

				Logger.LogTrace($"{TraceIdentifier} - Response sent");
			}

			return Task.CompletedTask;
		}

		public Task SendStatusResponseAsync(HttpStatusCode code)
		{
			Logger.LogTrace($"{TraceIdentifier} - Sending response: {new { RemoteEndPoint = request.RemoteEndPoint, ContentLength = ContentLength64.ToReadableSize(), StatusCode = response.StatusCode, ContentType = response.ContentType, ContentEncoding = response.ContentEncoding?.EncodingName }}");

			if (!OutputStream.CanWrite)
			{
				throw new InvalidOperationException("Output stream is not writable");
			}

			try
			{
				StatusCode = code;
				OutputStream.Close();
			}
			finally
			{
				ResponseSent = true;
				response.Close();

				Logger.LogTrace($"{TraceIdentifier} - Response sent");
			}

			return Task.CompletedTask;
		}

		public async Task SendResponseAsync(byte[] content)
		{
			if (content == null)
				throw new ArgumentNullException(nameof(content));


			if (!OutputStream.CanWrite)
			{
				throw new InvalidOperationException("Output stream is not writable");
			}

			Logger.LogTrace($"{TraceIdentifier} - Sending response: {new { RemoteEndPoint = request.RemoteEndPoint, ContentLength = content.ToReadableSize(), StatusCode = response.StatusCode, ContentType = response.ContentType, ContentEncoding = response.ContentEncoding?.EncodingName }}");

			try
			{
				await ResponseWriter.Write(content);
				response.OutputStream.Close();
			}
			catch
			{
				StatusCode = HttpStatusCode.InternalServerError;
				throw;
			}
			finally
			{
				ResponseSent = true;
				response.Close();

				Logger.LogTrace($"{TraceIdentifier} - Response sent");
			}
		}

		public async Task SendResponseAsync(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException(nameof(stream));

			if (!stream.CanRead)
			{
				throw new InvalidOperationException("Input stream is not readable");
			}

			if (!OutputStream.CanWrite)
			{
				throw new InvalidOperationException("Output stream is not writable");
			}

			Logger.LogTrace($"{TraceIdentifier} - Sending response: {new { RemoteEndPoint = request.RemoteEndPoint, ContentLength = stream.Length.ToReadableSize(), StatusCode = response.StatusCode, ContentType = response.ContentType, ContentEncoding = response.ContentEncoding?.EncodingName }}");

			try
			{
				await ResponseWriter.Write(stream);
				response.OutputStream.Close();
			}
			catch
			{
				StatusCode = HttpStatusCode.InternalServerError;
				throw;
			}
			finally
			{
				ResponseSent = true;
				response.Close();

				Logger.LogTrace($"{TraceIdentifier} - Response sent");
			}
		}
	}

	public static class HttpResponseExtensions
	{
		public static async Task SendHtmlResponseAsync(this HttpResponse response, string content)
		{
			if (response == null)
				throw new ArgumentNullException(nameof(response));

			if (content == null)
				throw new ArgumentNullException(nameof(content));

			response.ContentType = "text/html";
			var bytes = response.ContentEncoding.GetBytes(content);
			await response.SendResponseAsync(bytes);
		}

		public static async Task SendTextResponseAsync(this HttpResponse response, string content)
		{
			if (response == null)
				throw new ArgumentNullException(nameof(response));

			if (content == null)
				throw new ArgumentNullException(nameof(content));

			response.ContentType = "text/plain";
			var bytes = response.ContentEncoding.GetBytes(content);
			await response.SendResponseAsync(bytes);
		}

		public static async Task SendJsonResponseAsync<T>(this HttpResponse response, T content, JsonSerializerOptions options = null)
		{
			if (response == null)
				throw new ArgumentNullException(nameof(response));

			if (content == null)
				throw new ArgumentNullException(nameof(content));

			try
			{
				var json = options == null ?
											JsonSerializer.Serialize(content) :
											JsonSerializer.Serialize(content, options);

				response.ContentType = "application/json";
				var bytes = response.ContentEncoding.GetBytes(json);
				await response.SendResponseAsync(bytes);
			}
			catch
			{
				response.StatusCode = HttpStatusCode.InternalServerError;
				throw;
			}
			finally
			{
				response.CloseResponse();
			}
		}

		public static async Task SendFileResponseAsync(this HttpResponse response, string filename, ContentType contentType, ContentDisposition contentDisposition)
		{
			if (response == null)
				throw new ArgumentNullException(nameof(response));

			if (filename == null)
				throw new ArgumentNullException(nameof(filename));

			if (contentType == null)
				throw new ArgumentNullException(nameof(contentType));

			if (contentDisposition == null)
				throw new ArgumentNullException(nameof(contentDisposition));

			try
			{
				var file = new FileInfo(filename);
				using (var fs = file.OpenRead())
				{
					try
					{
						response.ContentType = contentType.MediaType;
						response.ContentDisposition = contentDisposition.DispositionType;
						await response.SendResponseAsync(fs);
					}
					finally
					{
						fs.Close();
					}
				}
			}
			catch
			{
				response.StatusCode = HttpStatusCode.InternalServerError;
				throw;
			}
			finally
			{
				response.CloseResponse();
			}
		}

		public static async Task SendFileResponseAsync(this HttpResponse response, string filename, string contentType, string contentDisposition)
		{
			if (response == null)
				throw new ArgumentNullException(nameof(response));

			if (filename == null)
				throw new ArgumentNullException(nameof(filename));

			if (contentType == null)
				throw new ArgumentNullException(nameof(contentType));

			if (contentDisposition == null)
				throw new ArgumentNullException(nameof(contentDisposition));

			try
			{
				var file = new FileInfo(filename);
				using (var fs = file.OpenRead())
				{
					try
					{
						response.ContentType = contentType;
						response.ContentDisposition = contentDisposition;
						await response.SendResponseAsync(fs);
					}
					finally
					{
						fs.Close();
					}
				}
			}
			catch
			{
				response.StatusCode = HttpStatusCode.InternalServerError;
				throw;
			}
			finally
			{
				response.CloseResponse();
			}
		}
	}
}
