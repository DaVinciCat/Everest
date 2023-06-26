using System;
using System.Net;
using System.Text;
using System.Text.Json;

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
		
		public byte[] Body { get; private set; } 

		private readonly HttpListenerResponse response;
			
		public HttpResponse(HttpListenerResponse response)
		{
			this.response = response ?? throw new ArgumentNullException(nameof(response));
			AppendHeader("Server", "Everest");
		}

		public void AddHeader(string name, string value) => response.AddHeader(name, value);

		public void AppendHeader(string name, string value) => response.AppendHeader(name, value);

		public void RemoveHeader(string name) => response.Headers.Remove(name);
		
		public void Write(HttpStatusCode code, string content)
		{
			Write(code, content, ContentEncoding);
		}
		
		public void Write(string content)
		{
			Write(HttpStatusCode.OK, content, ContentEncoding);
		}

		public void Write(HttpStatusCode code, string content, Encoding encoding)
		{
			StatusCode = code;
			Write(content, encoding);
		}

		public void Write(string content, Encoding encoding)
		{
			Body = encoding.GetBytes(content);
		}

		public void Write(byte[] content)
		{
			Body = content;
		}

		public void Send()
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
					ContentLength64 = Body.Length;
					response.OutputStream.Write(Body, 0, Body.Length);
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
		public static void Write200Ok(this HttpResponse response, string content)
		{
			if (response == null) throw 
				new ArgumentNullException(nameof(response));

			response.RemoveHeader("Content-Type");
			response.AppendHeader("Content-Type", "text/plain; charset=utf-8");
			response.Write(HttpStatusCode.OK, content);
		}

		public static void Write500InternalServerError(this HttpResponse response, string content)
		{
			if (response == null) 
				throw new ArgumentNullException(nameof(response));

			response.KeepAlive = false;
			response.RemoveHeader("Content-Type");
			response.AppendHeader("Content-Type", "text/plain; charset=utf-8");
			response.Write(HttpStatusCode.InternalServerError, content);
		}

		public static void Write400BadRequest(this HttpResponse response, string content)
		{
			if (response == null) 
				throw new ArgumentNullException(nameof(response));

			response.KeepAlive = false;
			response.RemoveHeader("Content-Type");
			response.AppendHeader("Content-Type", "text/plain; charset=utf-8");
			response.Write(HttpStatusCode.BadRequest, content);
		}

		public static void Write404NotFound(this HttpResponse response, string content)
		{
			if (response == null) 
				throw new ArgumentNullException(nameof(response));

			response.KeepAlive = false;
			response.RemoveHeader("Content-Type");
			response.AppendHeader("Content-Type", "text/plain; charset=utf-8");
			response.Write(HttpStatusCode.NotFound, content);
		}

		public static void Write401Unauthorized(this HttpResponse response, string content)
		{
			if (response == null) 
				throw new ArgumentNullException(nameof(response));

			response.KeepAlive = false;
			response.RemoveHeader("Content-Type");
			response.AppendHeader("Content-Type", "text/plain; charset=utf-8");
			response.Write(HttpStatusCode.Unauthorized, content);
		}

		public static void WriteJson<T>(this HttpResponse response, T content)
		{
			if (response == null) 
				throw new ArgumentNullException(nameof(response));

			response.WriteJson(HttpStatusCode.OK, content);
		}

		public static void WriteJson<T>(this HttpResponse response, HttpStatusCode code, T content)
		{
			if (response == null) 
				throw new ArgumentNullException(nameof(response));

			response.WriteJson(code, JsonSerializer.Serialize(content));
		}

		public static void WriteJson<T>(this HttpResponse response, HttpStatusCode code, T content, JsonSerializerOptions options)
		{
			if (response == null) 
				throw new ArgumentNullException(nameof(response));

			response.WriteJson(code, JsonSerializer.Serialize(content, options));
		}

		public static void WriteJson(this HttpResponse response, string json)
		{
			if (response == null) 
				throw new ArgumentNullException(nameof(response));

			response.WriteJson(HttpStatusCode.OK, json);
		}

		public static void WriteJson(this HttpResponse response, HttpStatusCode code, string json)
		{
			if (response == null) 
				throw new ArgumentNullException(nameof(response));

			response.RemoveHeader("Content-Type");
			response.AddHeader("Content-Type", "application/json");
			response.Write(code, json);
		}
	}
}
