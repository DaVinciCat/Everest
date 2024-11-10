using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Everest.Utils;
using Microsoft.Extensions.Logging;

namespace Everest.Http
{
    public interface IHttpResponse
    {
        Guid TraceIdentifier { get; }

        bool ResponseSent { get; }

        bool SendChunked { get; set; }

        long ContentLength64 { get; set; }

        bool KeepAlive { get; set; }

        string ContentType { get; set; }

        string ContentDisposition { get; set; }

        IPEndPoint RemoteEndPoint { get; }

        Encoding ContentEncoding { get; set; }

        NameValueCollection Headers { get; }

        HttpStatusCode StatusCode { get; set; }

        Stream OutputStream { get; }

        void SetCookie(Cookie cookie);

        bool HasHeader(string name);

        void AddHeader(string name, string value);

        void AppendHeader(string name, string value);

        void RemoveHeader(string name);

        void CloseResponse();

        Task RedirectAsync(string url);
    
        Task SendEmptyResponseAsync();

        Task SendStatusResponseAsync(HttpStatusCode code);

        Task SendResponseAsync(byte[] content);

        Task SendResponseAsync(byte[] content, int offset, int count);

        Task SendResponseAsync(Stream stream);
    }

    public static partial class HasLoggerExtensions
    {
        public static ILogger GetLogger(this IHttpResponse instance) => (instance as IHasLogger)?.Logger;
    }
}