using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Everest.Collections;
using Microsoft.Extensions.Logging;

namespace Everest.Http
{
    public class HttpRequestWrapper : IHttpRequest
    {
        public virtual ILogger<IHttpRequest> Logger => request.Logger;

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
        
        public virtual Task<byte[]> ReadRequestBodyAsync() => request.ReadRequestBodyAsync();
        
        private readonly IHttpRequest request;

        public HttpRequestWrapper(IHttpRequest request)
        {
            this.request = request;
        }
    }
}
