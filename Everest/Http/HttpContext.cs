using System.Net;
using Everest.Utils;

namespace Everest.Http
{
	public class HttpContext
	{
		public HttpRequest Request { get; }

		public HttpResponse Response { get; }

		public HttpContext(HttpListenerContext context)
		{
			Request = new HttpRequest(context.Request);
			Response = new HttpResponse(context.Response, new Compression(context.Request.GetAcceptEncodings()));
		}
	}
}
