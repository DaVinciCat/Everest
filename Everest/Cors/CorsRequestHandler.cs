using System;
using System.Collections.Generic;
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

		public CorsPolicy[] Policies => policies.Values.ToArray();

		private readonly Dictionary<string, CorsPolicy> policies = new();

		public string OriginHeader { get; set; } = "Origin";

		public CorsRequestHandler(ILogger<CorsRequestHandler> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public void AddCorsPolicy(CorsPolicy policy)
		{
			policies[policy.Origin] = policy;
		}

		public void RemoveCorsPolicy(CorsPolicy policy)
		{
			if (policies.ContainsKey(policy.Origin))
			{
				policies.Remove(policy.Origin);
			}
		}

		public void RemoveCorsPolice(string origin)
		{
			if (policies.ContainsKey(origin))
			{
				policies.Remove(origin);
			}
		}

		public void ClearCorsPolicies()
		{
			policies.Clear();
		}

		public Task<bool> TryHandleCorsRequestAsync(HttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			if (!context.Request.IsCorsPreflight())
			{
				return Task.FromResult(false);
			}

			Logger.LogTrace($"{context.Id} - Try to handle CORS preflight request");

			var origin = context.Request.Headers[OriginHeader];
			if (origin == null)
			{
				Logger.LogWarning($"{context.Id} - Failed to handle CORS preflight request. Missing header: {new { Header = OriginHeader }}");
				return Task.FromResult(true);
			}

			Logger.LogTrace($"{context.Id} - Try to apply CORS policy: {new { Request = context.Request.Description, Origin = origin, Policies = Policies.Select(p => p.Origin).ToReadableArray() }}");

			if (policies.TryGetValue(origin, out var policy))
			{
				var headers = new Headers(policy.AllowMethods, policy.AllowHeaders, policy.Origin, policy.MaxAge);
				context.Response.AddHeader("Access-Control-Allow-Methods", headers.AllowMethods);
				context.Response.AddHeader("Access-Control-Allow-Headers", headers.AllowHeaders);
				context.Response.AddHeader("Access-Control-Allow-Origin", headers.Origin);
				context.Response.AddHeader("Access-Control-Max-Age", headers.MaxAge);
				context.Response.StatusCode = HttpStatusCode.NoContent;

				Logger.LogTrace($"{context.Id} - Successfully handled CORS preflight request: {new { Policy = policy, AllowMethods = policy.AllowMethods.ToReadableArray(), AllowHeaders = policy.AllowHeaders.ToReadableArray(), Origin = policy.Origin, MaxAge = policy.MaxAge }}");
				return Task.FromResult(true);
			}

			Logger.LogWarning($"{context.Id} - Failed to handle CORS preflight request. Request contains no supported policy: {new { Origin = origin, Policies = Policies.Select(p => p.Origin).ToReadableArray() }}");
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
