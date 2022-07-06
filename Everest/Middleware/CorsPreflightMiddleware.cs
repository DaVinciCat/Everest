using Everest.Http;
using Everest.Utils;
using System.Net;

namespace Everest.Middleware
{
	/*
		https://developer.mozilla.org/en-US/docs/Glossary/Preflight_request
	*/
	public class CorsPreflightMiddleware : MiddlewareBase
	{
		public string[] AllowMethods { get; } = { "GET", "POST", "PUT", "DELETE" };

		public string[] AllowHeaders { get; } = { "Content-Type", "Accept", "X-Requested-With" };

		public string Origin { get; } = "*";

		public int MaxAge { get; } = 1728000;

		private readonly Headers headers;

		public CorsPreflightMiddleware(string[] allowMethods, string[] allowHeaders, string origin, int maxAge)
		{
			AllowMethods = allowMethods;
			AllowHeaders = allowHeaders;
			Origin = origin;
			MaxAge = maxAge;

			headers = new Headers(AllowMethods, AllowHeaders, Origin, MaxAge);
		}

		public CorsPreflightMiddleware()
		{
			headers = new Headers(AllowMethods, AllowHeaders, Origin, MaxAge);
		}

		public override void Invoke(HttpContext context)
		{
			if (HttpMethodIsOptions(context) &&
				HasAccessControlRequestMethodHeader(context) &&
				HasAccessControlRequestHeadersHeader(context) &&
				HasOriginHeader(context))
			{
				context.Response.AddHeader("Access-Control-Allow-Methods", headers.AllowMethods);
				context.Response.AddHeader("Access-Control-Allow-Headers", headers.AllowHeaders);
				context.Response.AddHeader("Access-Control-Allow-Origin", headers.Origin);
				context.Response.AddHeader("Access-Control-Max-Age", headers.MaxAge);

				context.Response.StatusCode = HttpStatusCode.NoContent;

				return;
			}

			if (HasNext)
				Next.Invoke(context);
		}

		private static bool HttpMethodIsOptions(HttpContext context)
		{
			return context.Request.HttpMethod == "OPTIONS";
		}

		private static bool HasAccessControlRequestMethodHeader(HttpContext context)
		{
			return context.Request.Headers["Access-Control-Request-Method"] != null;
		}

		private static bool HasAccessControlRequestHeadersHeader(HttpContext context)
		{
			return context.Request.Headers["Access-Control-Request-Headers"] != null;
		}

		private static bool HasOriginHeader(HttpContext context)
		{
			return context.Request.Headers["Origin"] != null;
		}

		private class Headers
		{
			public string AllowMethods { get; }

			public string AllowHeaders { get; }

			public string MaxAge { get; }

			public string Origin { get; }

			public Headers(string[] allowMethods, string[] allowHeaders, string origin, int maxAge)
			{
				AllowMethods = string.Join(", ", allowMethods);
				AllowHeaders = string.Join(", ", allowHeaders);
				Origin = origin;
				MaxAge = $"{maxAge}";
			}
		}
	}
}
