using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Everest.Collections;
using Everest.Utils;
using Microsoft.Extensions.Logging;

namespace Everest.Http
{
    public class HttpRequestProxy : IHttpRequest, IHasLogger
    {
        public virtual ILogger Logger => request.GetLogger();

        public virtual Guid TraceIdentifier => request.TraceIdentifier;

        public virtual string HttpMethod => request.HttpMethod;

        public virtual string Path => request.Path;

        public virtual string Description => request.Description;

        public virtual bool HasBody => request.HasBody;

        public virtual bool HasHeader(string name) => request.HasHeader(name);
        
        public virtual IPEndPoint RemoteEndPoint => request.RemoteEndPoint;

        public virtual Encoding ContentEncoding => request.ContentEncoding;

        public virtual NameValueCollection Headers => request.Headers;

        public virtual ParameterCollection QueryParameters
        {
            get => request.QueryParameters;
            set => request.QueryParameters = value;
        }

        public virtual ParameterCollection PathParameters
        {
            get => request.PathParameters;
            set => request.PathParameters = value;
        }

        public virtual Stream InputStream => request.InputStream;

        public virtual Task<byte[]> ReadBodyAsync() => request.ReadBodyAsync();
        
        private readonly IHttpRequest request;

        public HttpRequestProxy(IHttpRequest request)
        {
            this.request = request;
        }
    }
}
