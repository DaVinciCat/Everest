using System;
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

		public Guid TraceIdentifier => context.Request.RequestTraceIdentifier;

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

		public long ContentLength => pipe.Length;
		
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
			get => Headers["Content-Disposition"];
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					Headers.Remove("Content-Disposition");
				}
				else
				{
					Headers.Set("Content-Disposition", value);
				}
			}
		}

		public Encoding ContentEncoding
		{
			get => response.ContentEncoding ?? Encoding.UTF8;
			set => response.ContentEncoding = value;
		}

		public WebHeaderCollection Headers => response.Headers;

		public HttpStatusCode StatusCode
		{
			get => (HttpStatusCode)response.StatusCode;
			set
			{
				response.StatusCode = (int)value;
				response.StatusDescription = value.ToString();
			}
		}
		
		private readonly HttpListenerContext context;

		private readonly HttpListenerResponse response;

		private readonly StreamPipe pipe;

		public HttpResponse(HttpListenerContext context, ILogger<HttpResponse> logger)
		{
			this.context = context ?? throw new ArgumentNullException(nameof(context));
			response = context.Response;
			pipe = new(response.OutputStream);
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			StatusCode = HttpStatusCode.OK;
			ContentEncoding = Encoding.UTF8;
			
			AppendHeader("Server", "Everest");
		}

		public void AddHeader(string name, string value) => response.AddHeader(name, value);

		public void AppendHeader(string name, string value) => response.AppendHeader(name, value);

		public void RemoveHeader(string name) => response.Headers.Remove(name);

		public void PipeTo(Func<Stream, Stream> to) => pipe.PipeTo(to);
		
		public void PipeFrom(Stream from) => pipe.PipeFrom(from);

		public void PipeFrom(Func<Stream, Stream> from) => pipe.PipeFrom(from);

		public async Task SendResponseAsync()
		{
			try
			{
				Logger.LogTrace($"{TraceIdentifier} - Try to send response: {new { RemoteEndPoint = context.Request.RemoteEndPoint, ContentLength = ContentLength.ToReadableSize() }}");
				await pipe.FlushAsync();
				Logger.LogTrace($"{TraceIdentifier} - Successfully sended response: {new { RemoteEndPoint = context.Request.RemoteEndPoint, StatusCode = response.StatusCode, ContentType = response.ContentType, ContentEncoding = response.ContentEncoding?.EncodingName }}");
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, $"{TraceIdentifier} - Failed to send response");
			}
			finally
			{
				try
				{
					pipe.Dispose();
					response.OutputStream.Close();
					response.Close();
				}
				finally
				{
					ResponseSent = true;
					ResponseClosed = true;
					Logger.LogTrace($"{TraceIdentifier} - Response closed");
				}
			}
		}
	}

	public static class HttpResponseExtensions
	{
		public static Task WriteAsync(this HttpResponse response, byte[] content)
		{
			if (content == null)
				throw new ArgumentNullException(nameof(content));

			response.PipeFrom(new MemoryStream(content));
			return Task.CompletedTask;
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

			response.ContentType = "text/plain; charset=utf-8";
			await response.WriteAsync(content);
		}

		public static async Task WriteHtmlAsync(this HttpResponse response, string content)
		{
			if (response == null)
				throw new ArgumentNullException(nameof(response));

			if (content == null)
				throw new ArgumentNullException(nameof(content));

			response.ContentType = "text/html; charset=utf-8";
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
			response.ContentType = contentType.MediaType;
			response.ContentDisposition = contentDisposition.DispositionType;
			response.PipeFrom(file.OpenRead());

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
			response.ContentType = contentType;
			response.ContentDisposition = contentDisposition;
			response.PipeFrom(file.OpenRead());

			return Task.CompletedTask;
		}
	}
}
