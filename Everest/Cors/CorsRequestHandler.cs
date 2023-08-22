using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Utils;
using Microsoft.Extensions.Logging;

namespace Everest.Cors
{
    /*
	   https://developer.mozilla.org/en-US/docs/Glossary/Preflight_request
    */

    public class CorsRequestHandler : ICorsRequestHandler
	{
        public ILogger<CorsRequestHandler> Logger { get; }

        public CorsPolicyCollection Policies { get; } = new();
		
		public CorsRequestHandler(ILogger<CorsRequestHandler> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}
		
		public Task<bool> TryHandleCorsRequestAsync(HttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			if (!context.Request.IsCorsPreflightRequest())
			{
				Logger.LogWarning($"{context.TraceIdentifier} - Not a CORS preflight request");
				return Task.FromResult(false);
			}
			
			var origin = context.Request.Headers[HttpHeaders.Origin];
			if (origin == null)
			{
				Logger.LogWarning($"{context.TraceIdentifier} - Failed to handle CORS preflight request. Missing header: {new { Header = HttpHeaders.Origin }}");
				return Task.FromResult(true);
			}

			Logger.LogTrace($"{context.TraceIdentifier} - Try to match CORS policy: {new { Request = context.Request.Description, Origin = origin, Policies = Policies.Select(p => p.Origin).ToReadableArray() }}");

			if (Policies.TryGet(origin, out var policy))
			{
				var headers = new Headers(policy.AllowMethods, policy.AllowHeaders, policy.Origin, policy.MaxAge);
				context.Response.AddHeader(HttpHeaders.AccessControlAllowMethods, headers.AllowMethods);
				context.Response.AddHeader(HttpHeaders.AccessControlAllowHeaders, headers.AllowHeaders);
				context.Response.AddHeader(HttpHeaders.AccessControlAllowOrigin, headers.Origin);
				context.Response.AddHeader(HttpHeaders.AccessControlMaxAge, headers.MaxAge);
				Logger.LogTrace($"{context.TraceIdentifier} - Successfully matched CORS policy: {new { Policy = policy, AllowMethods = policy.AllowMethods.ToReadableArray(), AllowHeaders = policy.AllowHeaders.ToReadableArray(), Origin = policy.Origin, MaxAge = policy.MaxAge }}");
			}
			else
			{
				context.Response.AddHeader(HttpHeaders.AccessControlAllowMethods, "");
				context.Response.AddHeader(HttpHeaders.AccessControlAllowOrigin, "");
				context.Response.AddHeader(HttpHeaders.AccessControlMaxAge, "");
				Logger.LogWarning($"{context.TraceIdentifier} - Failed to match CORS policy. Request contains no supported policy: {new { Origin = origin, Policies = Policies.Select(p => p.Origin).ToReadableArray() }}");
			}

			context.Response.StatusCode = HttpStatusCode.NoContent;
			context.Response.Clear();
			return Task.FromResult(true);
		}

		#region Headers

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

		#endregion
	}
}
