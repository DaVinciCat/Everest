﻿using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Everest.Collections;
using Microsoft.Extensions.Logging;

namespace Everest.Http
{
    public interface IHttpRequest
    {
        ILogger<IHttpRequest> Logger { get; }

        Guid TraceIdentifier { get; }

        string HttpMethod { get; }

        string Path { get; }

        string Description { get; }

        bool HasRequestBody { get; }

        bool HasHeader(string name);

        IPEndPoint RemoteEndPoint { get; }
    
        Encoding ContentEncoding { get; }

        NameValueCollection Headers { get; }

        ParameterCollection QueryParameters { get; set; }

        ParameterCollection PathParameters { get; set; }
    
        Stream InputStream { get; }

        Task<byte[]> ReadRequestBodyAsync();
    }
}