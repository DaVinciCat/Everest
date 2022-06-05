using Everest.Utils;
using System;
using System.IO;
using System.Net;
using System.Text;

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

		internal Stream OutputStream => response.OutputStream;
		
		public byte[] Body { get; private set; } 

		private readonly HttpListenerResponse response;
			
		public HttpResponse(HttpListenerResponse response)
		{
			this.response = response;
			AppendHeader("Server", "Everest");
		}

		public void AddHeader(string name, string value) => response.AddHeader(name, value);

		public void AppendHeader(string name, string value) => response.AppendHeader(name, value);

		public void RemoveHeader(string name) => response.Headers.Remove(name);

		public void Write(string content, HttpStatusCode code)
		{
			Write(content, ContentEncoding, code);
		}

		public void Write(string content, Encoding encoding, HttpStatusCode code)
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

				if (!OutputStream.CanWrite)
					throw new NotSupportedException("Response stream does not support writing.");

				if (Body != null)
				{
					ContentLength64 = Body.Length;
					OutputStream.Write(Body, 0, Body.Length);
				}
				
				IsSent = true;
			}
			finally
			{
				if (OutputStream.CanWrite)
					OutputStream.Close();

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
			response.RemoveHeader("Content-Type");
			response.AppendHeader("Content-Type", "text/plain; charset=utf-8");
			response.Write(content, HttpStatusCode.OK);
		}

		public static void Write500InternalServerError(this HttpResponse response, string content)
		{
			response.KeepAlive = false;
			response.RemoveHeader("Content-Type");
			response.AppendHeader("Content-Type", "text/plain; charset=utf-8");
			response.Write(content, HttpStatusCode.InternalServerError);
		}

		public static void Write400BadRequest(this HttpResponse response, string content)
		{
			response.KeepAlive = false;
			response.RemoveHeader("Content-Type");
			response.AppendHeader("Content-Type", "text/plain; charset=utf-8");
			response.Write(content, HttpStatusCode.BadRequest);
		}

		public static void Write404NotFound(this HttpResponse response, string content)
		{
			response.KeepAlive = false;
			response.RemoveHeader("Content-Type");
			response.AppendHeader("Content-Type", "text/plain; charset=utf-8");
			response.Write(content, HttpStatusCode.NotFound);
		}

		public static void WriteJson<T>(this HttpResponse response, T content)
		{
			response.WriteJson(content, HttpStatusCode.OK);
		}

		public static void WriteJson<T>(this HttpResponse response, T content, HttpStatusCode code)
		{
			response.RemoveHeader("Content-Type");
			response.AddHeader("Content-Type", "application/json");
			response.Write(content.ToJson(), code);
		}
	}
}
