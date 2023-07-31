﻿using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Everest.Http
{
	public class HttpResponse
	{
		public bool ResponseSent { get; private set; }

		public bool ResponseClosed { get; private set; }

		public bool SendChunked
		{
			get => response.SendChunked;
			set => response.SendChunked = value;
		}

		/*https://learn.microsoft.com/ru-ru/dotnet/api/system.net.httplistenerresponse.contentlength64?view=net-7.0*/
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

		public Stream Input { get; set; } = new MemoryStream();

		public Stream Output { get; set; }

		public Stream OutputStream => response.OutputStream;

		private readonly HttpListenerResponse response;

		public HttpResponse(HttpListenerResponse response)
		{
			this.response = response ?? throw new ArgumentNullException(nameof(response));

			Output = new BufferedStream(response.OutputStream);
			StatusCode = HttpStatusCode.OK;
			AppendHeader("Server", "Everest");
		}

		public void AddHeader(string name, string value) => response.AddHeader(name, value);

		public void AppendHeader(string name, string value) => response.AppendHeader(name, value);

		public void RemoveHeader(string name) => response.Headers.Remove(name);

		public async Task SendResponseAsync()
		{
			try
			{
				Input.Position = 0;
		
				var buffer = new byte[4 * 1024];
				int read;
				while ((read = await Input.ReadAsync(buffer, 0, buffer.Length)) > 0)
				{
					await Output.WriteAsync(buffer, 0, read);
					await Output.FlushAsync();
				}
			}
			finally
			{
				try
				{
					Input.Close();
					Output.Close();
					response.OutputStream.Close();
					response.Close();
				}
				finally
				{
					ResponseSent = true;
					ResponseClosed = true;
				}
			}
		}
	}

	public static class HttpResponseExtensions
	{
		public static async Task WriteAsync(this HttpResponse response, byte[] content)
		{
			if (content == null)
				throw new ArgumentNullException(nameof(content));

			await response.Input.WriteAsync(content, 0, content.Length);
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

		public static async Task WriteTextAsync(HttpResponse response, string content)
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

		public static async Task WriteFileAsync(this HttpResponse response, string filename, ContentType contentType, ContentDisposition contentDisposition)
		{
			var file = new FileInfo(filename);
			response.ContentType = contentType.MediaType;
			response.ContentDisposition = contentDisposition.DispositionType;
			response.Input = file.OpenRead();

			await Task.CompletedTask;
		}

		public static async Task WriteFileAsync(this HttpResponse response, string filename, string contentType, string contentDisposition)
		{
			var file = new FileInfo(filename);
			response.ContentType = contentType;
			response.ContentDisposition = contentDisposition;
			response.Input = file.OpenRead();

			await Task.CompletedTask;
		}
	}
}
