using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Everest.Http
{
	public class HttpResponse
	{
		public bool ResponseSent { get; private set; }

		public bool ResponseClosed { get; private set; }

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

		public string ContentDisposition { get; set; }

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
		
		public Stream OutputStream { get; } = new MemoryStream();

		private readonly HttpListenerResponse response;

		public HttpResponse(HttpListenerResponse response)
		{
			this.response = response ?? throw new ArgumentNullException(nameof(response));

			StatusCode = HttpStatusCode.OK;
			AppendHeader("Server", "Everest");
		}

		public void AddHeader(string name, string value) => response.AddHeader(name, value);

		public void AppendHeader(string name, string value) => response.AppendHeader(name, value);

		public void RemoveHeader(string name) => response.Headers.Remove(name);

		public async Task SendResponseAsync(Stream stream)
		{
			try
			{
				stream.Position = 0;
				ContentLength64 = stream.Length;

				using (var br = new BinaryReader(stream, ContentEncoding, true))
				{
					var buffer = new byte[4 * 1024];
					int read;
					while ((read = br.Read(buffer, 0, buffer.Length)) > 0)
					{
						await response.OutputStream.WriteAsync(buffer, 0, read);
						await response.OutputStream.FlushAsync();
					}
					
					br.Close();
				}
			}
			finally
			{
				try
				{
					stream.Close();
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

		public async Task SendFileAsync(string filename, string contentType, string contentDisposition)
		{
			try
			{
				var file = new FileInfo(filename);
				using (var fs = file.OpenRead())
				{
					ContentType = contentType;
					ContentDisposition = contentDisposition;
					RemoveHeader("Content-disposition");
					AddHeader("Content-disposition", contentDisposition);

					using (var bw = new BinaryWriter(response.OutputStream, ContentEncoding, true))
					{
						var buffer = new byte[4 * 1024];
						int read;
						while ((read = await fs.ReadAsync(buffer, 0, buffer.Length)) > 0)
						{
							await response.OutputStream.WriteAsync(buffer, 0, read);
							await response.OutputStream.FlushAsync();
						}

						bw.Close();
					}

					fs.Close();
				}
			}
			finally
			{
				try
				{
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
			if (response == null)
				throw new ArgumentNullException(nameof(response));

			if (content == null)
				throw new ArgumentNullException(nameof(content));
			
			await response.OutputStream.WriteAsync(content, 0, content.Length);
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
			if (response == null) throw
				new ArgumentNullException(nameof(response));

			if (content == null)
				throw new ArgumentNullException(nameof(content));

			response.RemoveHeader("Content-Type");
			response.AppendHeader("Content-Type", "text/plain; charset=utf-8");

			await response.WriteAsync(content);
		}

		public static async Task WriteHtmlAsync(this HttpResponse response, string content)
		{
			if (response == null) throw
				new ArgumentNullException(nameof(response));

			if (content == null)
				throw new ArgumentNullException(nameof(content));

			response.RemoveHeader("Content-Type");
			response.AppendHeader("Content-Type", "text/html; charset=utf-8");

			await response.WriteAsync(content);
		}

		public static async Task WriteJsonAsync<T>(this HttpResponse response, T content, JsonSerializerOptions options = null)
		{
			if (response == null)
				throw new ArgumentNullException(nameof(response));

			if (content == null)
				throw new ArgumentNullException(nameof(content));

			response.RemoveHeader("Content-Type");
			response.AddHeader("Content-Type", "application/json");

			var json = options == null ?
				JsonSerializer.Serialize(content) :
				JsonSerializer.Serialize(content, options);

			await response.WriteAsync(json);
		}

		public static async Task WriteFileAsync(this HttpResponse response, string filename, string contentType, string contentDisposition)
		{
			var file = new FileInfo(filename);
			using (var fs = file.OpenRead())
			{
				response.ContentType = contentType;
				response.ContentDisposition = contentDisposition;
				response.RemoveHeader("Content-disposition");
				response.AddHeader("Content-disposition", contentDisposition);

				using (var bw = new BinaryWriter(response.OutputStream, response.ContentEncoding, true))
				{
					var buffer = new byte[4 * 1024];
					int read;
					while ((read = await fs.ReadAsync(buffer, 0, buffer.Length)) > 0)
					{
						await response.OutputStream.WriteAsync(buffer, 0, read);
						await response.OutputStream.FlushAsync();
					}

					bw.Close();
				}

				fs.Close();
			}
		}
	}
}
