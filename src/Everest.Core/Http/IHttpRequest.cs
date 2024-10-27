using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Everest.Common.Collections;
using Everest.Common.Logging;
using Microsoft.Extensions.Logging;

namespace Everest.Core.Http
{
    public interface IHttpRequest
    {
        Guid TraceIdentifier { get; }

        string HttpMethod { get; }

        string Path { get; }

        string Description { get; }

        bool HasBody { get; }

        bool HasHeader(string name);

        IPEndPoint RemoteEndPoint { get; }
    
        Encoding ContentEncoding { get; }

        NameValueCollection Headers { get; }

        ParameterCollection QueryParameters { get; set; }

        ParameterCollection PathParameters { get; set; }
    
        Stream InputStream { get; }

        Task<byte[]> ReadBodyAsync();
    }

    public static partial class HasLoggerExtensions
    {
        public static ILogger GetLogger(this IHttpRequest instance) => (instance as IHasLogger)?.Logger;
    }
}