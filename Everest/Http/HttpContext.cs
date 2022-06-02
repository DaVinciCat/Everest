using System;

namespace Everest.Http
{
	public class HttpContext
	{
		public HttpRequest Request { get; }

		public HttpResponse Response { get; }

		public IServiceProvider Services { get; }

		public HttpContext(HttpRequest request, HttpResponse response, IServiceProvider services)
		{
			Request = request;
			Response = response;
			Services = services;
		}
	}
}
