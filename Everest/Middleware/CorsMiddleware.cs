using Everest.Http;
using Everest.Utils;

namespace Everest.Middleware
{
	public class CorsMiddleware : MiddlewareBase
	{
		public string[] AllowHeaders { get; } = { "Content-Type", "Accept", "X-Requested-With" };

		public string Origin { get; } = "*";

		private readonly Headers headers;

		public CorsMiddleware(string[] allowHeaders, string origin)
		{
			AllowHeaders = allowHeaders;
			Origin = origin;

			headers = new Headers(AllowHeaders, Origin);
		}

		public CorsMiddleware()
		{
			headers = new Headers(AllowHeaders, Origin);
		}

		public override void Invoke(HttpContext context)
		{
			context.Response.AddHeader("Access-Control-Allow-Headers", headers.AllowHeaders);
			context.Response.AddHeader("Access-Control-Allow-Origin", headers.Origin);

			if (HasNext)
				Next.Invoke(context);
		}

		private class Headers
		{
			public string AllowHeaders { get; }

			public string Origin { get; }

			public Headers(string[] allowHeaders, string origin)
			{
				AllowHeaders = string.Join(", ", allowHeaders);
				Origin = origin;
			}
		}
	}
}
