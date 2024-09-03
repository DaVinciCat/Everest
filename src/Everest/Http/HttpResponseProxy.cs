using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Everest.Http
{
    public class HttpResponseProxy : IHttpResponse
    {
        public virtual ILogger<IHttpResponse> Logger => response.Logger;

        public virtual Guid TraceIdentifier => response.TraceIdentifier;

        public virtual bool ResponseSent => response.ResponseSent;

        public virtual bool SendChunked
        {
            get => response.SendChunked;
            set => response.SendChunked = value;
        }

        public virtual long ContentLength64
        {
            get => response.ContentLength64;
            set => response.ContentLength64 = value;
        }

        public virtual bool KeepAlive
        {
            get => response.KeepAlive;
            set => response.KeepAlive = value;
        }

        public virtual string ContentType
        {
            get => response.ContentType;
            set => response.ContentType = value;
        }

        public virtual string ContentDisposition
        {
            get => response.ContentDisposition;
            set => response.ContentDisposition = value;
        }

        public virtual IPEndPoint RemoteEndPoint => response.RemoteEndPoint;

        public virtual Encoding ContentEncoding
        {
            get => response.ContentEncoding;
            set => response.ContentEncoding = value;
        }

        public virtual NameValueCollection Headers => response.Headers;

        public virtual HttpStatusCode StatusCode
        {
            get => response.StatusCode;
            set => response.StatusCode = value;
        }

        public virtual Stream OutputStream => response.OutputStream;

        public virtual void SetCookie(Cookie cookie) => response.SetCookie(cookie);

        public virtual bool HasHeader(string name) => response.HasHeader(name);

        public virtual void AddHeader(string name, string value) => response.AddHeader(name, value);

        public virtual void AppendHeader(string name, string value) => response.AppendHeader(name, value);

        public virtual void RemoveHeader(string name) => response.RemoveHeader(name);

        public virtual void CloseResponse() => response.CloseResponse();

        public virtual Task RedirectAsync(string url) => response.RedirectAsync(url);

        public virtual Task SendEmptyResponseAsync() => response.SendEmptyResponseAsync();

        public virtual Task SendStatusResponseAsync(HttpStatusCode code) => response.SendStatusResponseAsync(code);

        public virtual Task SendResponseAsync(byte[] content) => response.SendResponseAsync(content);

        public virtual Task SendResponseAsync(byte[] content, int offset, int count) => response.SendResponseAsync(content, offset, count);

        public virtual Task SendResponseAsync(Stream stream) => response.SendResponseAsync(stream);

        private readonly IHttpResponse response;

        public HttpResponseProxy(IHttpResponse response)
        {
            this.response = response;
        }
    }
}
