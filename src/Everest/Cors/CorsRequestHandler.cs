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

        public CorsPolicyCollection Policies { get; } = new CorsPolicyCollection();
		
		public CorsRequestHandler(ILogger<CorsRequestHandler> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}
		
		public async Task<bool> TryHandleCorsRequestAsync(IHttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			if(context.Response.ResponseSent)
				return false;

			if (!context.Request.IsCorsPreflightRequest())
			{
                Logger.LogWarningIfEnabled(() => $"{context.TraceIdentifier} - Not a CORS preflight request");
				return false;
			}
			
			var origin = context.Request.Headers[HttpHeaders.Origin];
			if (origin == null)
			{
                Logger.LogWarningIfEnabled(() => $"{context.TraceIdentifier} - Failed to handle CORS preflight request. Missing header: {new { Header = HttpHeaders.Origin }}");
                return false;
			}

            Logger.LogTraceIfEnabled(() => $"{context.TraceIdentifier} - Try to match CORS policy: {new { Request = context.Request.Description, Origin = origin, Policies = Policies.Select(p => p.Origin).ToReadableArray() }}");

			if (Policies.TryGet(origin, out var policy))
			{
				var headers = new Headers(policy.AllowMethods, policy.AllowHeaders, policy.Origin, policy.MaxAge);
				context.Response.AddHeader(HttpHeaders.AccessControlAllowMethods, headers.AllowMethods);
				context.Response.AddHeader(HttpHeaders.AccessControlAllowHeaders, headers.AllowHeaders);
				context.Response.AddHeader(HttpHeaders.AccessControlAllowOrigin, headers.Origin);
				context.Response.AddHeader(HttpHeaders.AccessControlMaxAge, headers.MaxAge);

                Logger.LogTraceIfEnabled(() => $"{context.TraceIdentifier} - Successfully matched CORS policy: {new { Policy = policy, AllowMethods = policy.AllowMethods.ToReadableArray(), AllowHeaders = policy.AllowHeaders.ToReadableArray(), Origin = policy.Origin, MaxAge = policy.MaxAge }}");
			}
			else
			{
				context.Response.AddHeader(HttpHeaders.AccessControlAllowMethods, "");
				context.Response.AddHeader(HttpHeaders.AccessControlAllowOrigin, "");
				context.Response.AddHeader(HttpHeaders.AccessControlMaxAge, "");

                Logger.LogWarningIfEnabled(() => $"{context.TraceIdentifier} - Failed to match CORS policy. Request contains no supported policy: {new { Origin = origin, Policies = Policies.Select(p => p.Origin).ToReadableArray() }}");
			}

			await context.Response.SendStatusResponseAsync(HttpStatusCode.NoContent);
			return true;
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
