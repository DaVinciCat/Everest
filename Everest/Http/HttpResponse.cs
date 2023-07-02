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
		
        public Stream OutputStream { get; set; } = new MemoryStream();

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
		
        public async Task SendResponceAsync()
        {
			try
			{
				if (OutputStream != null)
				{
					OutputStream.Position = 0;
					using (var reader = new BinaryReader(OutputStream, ContentEncoding))
					{
						var bytes = reader.ReadBytes((int)OutputStream.Length);
						response.ContentLength64 = bytes.Length;
						await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
						await response.OutputStream.FlushAsync();
						response.OutputStream.Close();
					}
				}
			}
			finally
			{
				try
				{
					response.Close();
				}
				finally
				{
					ResponseSent = true;
				}
			}
        }
    }

    public static class HttpResponseExtensions
	{
		public static async Task WriteAsync(this HttpResponse response, byte[] content)
		{
			await response.OutputStream.WriteAsync(content, 0, content.Length);
		}

        public static async Task WriteAsync(this HttpResponse response, string content)
		{
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
	}
}
