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

		public Stream OutputStream => response.OutputStream;

		private readonly HttpListenerResponse response;

		private readonly Compression compression;

		public HttpResponse(HttpListenerResponse response, Compression compression)
		{
			this.response = response;
			this.compression = compression;

			AppendHeader("Server", "Everest");
		}

		public void AddHeader(string name, string value) => response.AddHeader(name, value);

		public void AppendHeader(string name, string value) => response.AppendHeader(name, value);

		public void RemoveHeader(string name) => response.Headers.Remove(name);

		public void SendOk(string content)
		{
			RemoveHeader("Content-Type");
			AppendHeader("Content-Type", "text; charset=utf-8");
			Send(content, HttpStatusCode.OK);
		}

		public void SendInternalServerError(string content)
		{
			KeepAlive = false;
			RemoveHeader("Content-Type");
			AppendHeader("Content-Type", "text; charset=utf-8");
			Send(content, HttpStatusCode.InternalServerError);
		}

		public void SendBadRequest(string content)
		{
			KeepAlive = false;
			RemoveHeader("Content-Type");
			AppendHeader("Content-Type", "text; charset=utf-8");
			Send(content, HttpStatusCode.BadRequest);
		}

		public void SendNotFound(string content)
		{
			KeepAlive = false;
			RemoveHeader("Content-Type");
			AppendHeader("Content-Type", "text; charset=utf-8");
			Send(content, HttpStatusCode.NotFound);
		}

		public void SendJson<T>(T content)
		{
			SendJson(content, HttpStatusCode.OK);
		}

		public void SendJson<T>(T content, HttpStatusCode code)
		{
			RemoveHeader("Content-Type");
			AddHeader("Content-Type", "application/json");
			Send(content.ToJson(), code);
		}

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
			if (buffer.Length >= compression.CompressionMinLength)
			{
				if (compression.TryCompress(ref buffer, out var enc))
				{
					RemoveHeader("Content-Encoding");
					AddHeader("Content-Encoding", enc);
				}
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
}
