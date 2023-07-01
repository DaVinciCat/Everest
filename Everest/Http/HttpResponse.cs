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
        public bool IsSent { get; private set; }

        public bool IsClosed { get; private set; }

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

        public long ContentLength64
        {
            get => response.ContentLength64;
            set => response.ContentLength64 = value;
        }

        public Stream Body { get; } = new MemoryStream();

        private readonly HttpListenerResponse response;

        public HttpResponse(HttpListenerResponse response)
        {
            this.response = response ?? throw new ArgumentNullException(nameof(response));
            AppendHeader("Server", "Everest");
        }

        public void AddHeader(string name, string value) => response.AddHeader(name, value);

        public void AppendHeader(string name, string value) => response.AppendHeader(name, value);

        public void RemoveHeader(string name) => response.Headers.Remove(name);

        public Task WriteAsync(HttpStatusCode code, string content)
        {
	        if (content == null) 
		        throw new ArgumentNullException(nameof(content));

	        return WriteAsync(code, content, ContentEncoding);
        }

        public Task WriteAsync(string content)
        {
	        if (content == null) 
		        throw new ArgumentNullException(nameof(content));

	        return WriteAsync(HttpStatusCode.OK, content, ContentEncoding);
        }

        public async Task WriteAsync(HttpStatusCode code, string content, Encoding encoding)
        {
	        if (content == null) 
		        throw new ArgumentNullException(nameof(content));
	       
	        if (encoding == null) 
		        throw new ArgumentNullException(nameof(encoding));

	        StatusCode = code;
            await WriteAsync(content, encoding);
        }

        public async Task WriteAsync(string content, Encoding encoding)
        {
	        if (content == null)
		        throw new ArgumentNullException(nameof(content));
	       
	        if (encoding == null) 
		        throw new ArgumentNullException(nameof(encoding));

	        var bytes = encoding.GetBytes(content);
	        await WriteAsync(bytes);
        }
        
        public async Task WriteAsync(byte[] content)
        {
	        if (content == null)
		        throw new ArgumentNullException(nameof(content));

	        using (var ms = new MemoryStream(content))
	        {
		        await WriteAsync(ms);
	        }
		}

        public async Task WriteAsync(Stream content)
        {
	        if (content == null)
		        throw new ArgumentNullException(nameof(content));

	        await FlushAsync();
	        await content.CopyToAsync(Body);
	        Body.Position = 0;
        }

        public async Task FlushAsync()
        {
	        if (Body.Length > 0)
	        {
		        await Body.FlushAsync();
	        }
        }

        public async Task SendAsync()
        {
            try
            {
                if (IsSent)
                    throw new InvalidOperationException("Response is already sent.");

                if (IsClosed)
                    throw new InvalidOperationException("Response is closed.");

                if (!response.OutputStream.CanWrite)
                    throw new NotSupportedException("Response stream does not support writing.");

				if (Body != null)
				{
					Body.Position = 0;
					await Body.CopyToAsync(response.OutputStream);
				}

				IsSent = true;
            }
            finally
            {
                if (response.OutputStream.CanWrite)
                    response.OutputStream.Close();

                Close();
            }
        }

        public void Close()
        {
            if (!IsClosed)
                response.Close();

            IsClosed = true;
        }
    }

    public static class HttpResponseExtensions
	{
		public static async Task Write200OkAsync(this HttpResponse response, string content)
		{
			if (response == null) throw 
				new ArgumentNullException(nameof(response));
			
			if (content == null) 
				throw new ArgumentNullException(nameof(content));

			response.RemoveHeader("Content-Type");
			response.AppendHeader("Content-Type", "text/plain; charset=utf-8");
			await response.WriteAsync(HttpStatusCode.OK, content);
		}

		public static async Task Write500InternalServerErrorAsync(this HttpResponse response, string content)
		{
			if (response == null) 
				throw new ArgumentNullException(nameof(response));
			
			if (content == null) 
				throw new ArgumentNullException(nameof(content));

			response.KeepAlive = false;
			response.RemoveHeader("Content-Type");
			response.AppendHeader("Content-Type", "text/plain; charset=utf-8");
			await response.WriteAsync(HttpStatusCode.InternalServerError, content);
		}

		public static async Task Write400BadRequestAsync(this HttpResponse response, string content)
		{
			if (response == null) 
				throw new ArgumentNullException(nameof(response));
			
			if (content == null) 
				throw new ArgumentNullException(nameof(content));

			response.KeepAlive = false;
			response.RemoveHeader("Content-Type");
			response.AppendHeader("Content-Type", "text/plain; charset=utf-8");
			await response.WriteAsync(HttpStatusCode.BadRequest, content);
		}

		public static async Task Write404NotFoundAsync(this HttpResponse response, string content)
		{
			if (response == null) 
				throw new ArgumentNullException(nameof(response));
			
			if (content == null) 
				throw new ArgumentNullException(nameof(content));

			response.KeepAlive = false;
			response.RemoveHeader("Content-Type");
			response.AppendHeader("Content-Type", "text/plain; charset=utf-8");
			await response.WriteAsync(HttpStatusCode.NotFound, content);
		}

		public static async Task Write401UnauthorizedAsync(this HttpResponse response, string content)
		{
			if (response == null) 
				throw new ArgumentNullException(nameof(response));
			
			if (content == null) 
				throw new ArgumentNullException(nameof(content));

			response.KeepAlive = false;
			response.RemoveHeader("Content-Type");
			response.AppendHeader("Content-Type", "text/plain; charset=utf-8");
			await response.WriteAsync(HttpStatusCode.Unauthorized, content);
		}

		public static async Task WriteJsonAsync<T>(this HttpResponse response, T content)
		{
			if (response == null) 
				throw new ArgumentNullException(nameof(response));
			
			if (content == null) 
				throw new ArgumentNullException(nameof(content));

			await response.WriteJsonAsync(HttpStatusCode.OK, content);
		}

		public static async Task WriteJsonAsync<T>(this HttpResponse response, HttpStatusCode code, T content)
		{
			if (response == null) 
				throw new ArgumentNullException(nameof(response));
			
			if (content == null) 
				throw new ArgumentNullException(nameof(content));

			await response.WriteJsonAsync(code, JsonSerializer.Serialize(content));
		}

		public static async Task WriteJsonAsync<T>(this HttpResponse response, HttpStatusCode code, T content, JsonSerializerOptions options)
		{
			if (response == null) 
				throw new ArgumentNullException(nameof(response));
			
			if (content == null) 
				throw new ArgumentNullException(nameof(content));
			
			if (options == null) 
				throw new ArgumentNullException(nameof(options));

			await response.WriteJsonAsync(code, JsonSerializer.Serialize(content, options));
		}

		public static async Task WriteJsonAsync(this HttpResponse response, string json)
		{
			if (response == null) 
				throw new ArgumentNullException(nameof(response));
			
			if (json == null) 
				throw new ArgumentNullException(nameof(json));

			await response.WriteJsonAsync(HttpStatusCode.OK, json);
		}

		public static async Task WriteJsonAsync(this HttpResponse response, HttpStatusCode code, string json)
		{
			if (response == null) 
				throw new ArgumentNullException(nameof(response));
			
			if (json == null) 
				throw new ArgumentNullException(nameof(json));

			response.RemoveHeader("Content-Type");
			response.AddHeader("Content-Type", "application/json");
			await response.WriteAsync(code, json);
		}
	}
}
