using Everest.Utils;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Everest.Http
{
	public class HttpResponse
	{
		public bool ResponseSent { get; private set; }

		public bool ResponseClosed { get; private set; }

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

		public ICompression Compression { get; }

		public Stream OutputStream => response.OutputStream;

		private readonly HttpListenerResponse response;
			
		public HttpResponse(HttpListenerResponse response, ICompression compression)
		{
			this.response = response;
			Compression = compression;
			AppendHeader("Server", "Everest");
		}

		public void AddHeader(string name, string value) => response.AddHeader(name, value);

		public void AppendHeader(string name, string value) => response.AppendHeader(name, value);

		public void RemoveHeader(string name) => response.Headers.Remove(name);

		public void Send(string content, HttpStatusCode code)
		{
			Send(content, ContentEncoding, code);
		}

		public void Send(string content, Encoding encoding, HttpStatusCode code)
		{
			StatusCode = code;
			Send(content, encoding);
		}

		public void Send(string content, Encoding encoding)
		{
			var buffer = encoding.GetBytes(content);
			if (Compression.TryCompress(ref buffer, out var enc))
			{
				RemoveHeader("Content-Encoding");
				AddHeader("Content-Encoding", enc);
			}

			Send(buffer);
		}

		public void Send(byte[] content)
		{
			try
			{
				if (ResponseSent)
					throw new InvalidOperationException("Response is already sent.");

				if (ResponseClosed)
					throw new InvalidOperationException("Response is closed.");

				if (!OutputStream.CanWrite)
					throw new NotSupportedException("Response stream does not support writing.");

				ContentLength64 = content.Length;
				OutputStream.Write(content, 0, content.Length);

				ResponseSent = true;
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
			if (!ResponseClosed)
				response.Close();

			ResponseClosed = true;
		}
	}

	public static class HttpResponseExtensions
	{
		public static void Send200Ok(this HttpResponse response, string content)
		{
			response.RemoveHeader("Content-Type");
			response.AppendHeader("Content-Type", "text/plain; charset=utf-8");
			response.Send(content, HttpStatusCode.OK);
		}

		public static void Send500InternalServerError(this HttpResponse response, string content)
		{
			response.KeepAlive = false;
			response.RemoveHeader("Content-Type");
			response.AppendHeader("Content-Type", "text/plain; charset=utf-8");
			response.Send(content, HttpStatusCode.InternalServerError);
		}

		public static void Send400BadRequest(this HttpResponse response, string content)
		{
			response.KeepAlive = false;
			response.RemoveHeader("Content-Type");
			response.AppendHeader("Content-Type", "text/plain; charset=utf-8");
			response.Send(content, HttpStatusCode.BadRequest);
		}

		public static void Send404NotFound(this HttpResponse response, string content)
		{
			response.KeepAlive = false;
			response.RemoveHeader("Content-Type");
			response.AppendHeader("Content-Type", "text/plain; charset=utf-8");
			response.Send(content, HttpStatusCode.NotFound);
		}

		public static void SendJson<T>(this HttpResponse response, T content)
		{
			response.SendJson(content, HttpStatusCode.OK);
		}

		public static void SendJson<T>(this HttpResponse response, T content, HttpStatusCode code)
		{
			response.RemoveHeader("Content-Type");
			response.AddHeader("Content-Type", "application/json");
			response.Send(content.ToJson(), code);
		}
	}
}
