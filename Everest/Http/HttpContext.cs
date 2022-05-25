using System;
using System.Net;

namespace Everest.Http
{
	public class HttpContext
	{
		public HttpRequest Request { get; }

		public HttpResponse Response { get; }

		public IServiceProvider Services { get; internal set; }

		public HttpContext(HttpListenerContext context)
		{
			//TODO: super naive implementation, should replace it with q values support
			var acceptEncoding = context.Request.Headers["Accept-Encoding"] ?? string.Empty;
			var encodings = acceptEncoding.Split(','); 
			
			Request = new HttpRequest(context.Request); 
			Response = new HttpResponse(context.Response, new Compression(encodings));
		}
	}
}
