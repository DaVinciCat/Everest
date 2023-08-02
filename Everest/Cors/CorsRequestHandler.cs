﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Routing;
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

		public void RemoveCorsPolicy(string origin)
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

			Logger.LogTrace($"{context.TraceIdentifier} - Try to check if CORS preflight request");

			if (!context.Request.IsCorsPreflightRequest())
			{
				Logger.LogTrace($"{context.TraceIdentifier} - Not a CORS preflight request");
				return Task.FromResult(false);
			}

			if (context.Response.ResponseSent)
			{
				Logger.LogTrace($"{context.TraceIdentifier} - No CORS policy handling required. Response is already sent");
				return Task.FromResult(false);
			}
			
			if (context.TryGetRouteDescriptor(out var descriptor))
			{
				Logger.LogTrace($"{context.TraceIdentifier} - No CORS policy handling required. CORS request was handled by: {new { Route = descriptor.Route.Description, EndPoint = descriptor.EndPoint.Description }}");
				return Task.FromResult(false);
			}
			
			Logger.LogTrace($"{context.TraceIdentifier} - Try to handle CORS preflight request");

			var origin = context.Request.Headers[HttpHeaders.Origin];
			if (origin == null)
			{
				Logger.LogTrace($"{context.TraceIdentifier} - Failed to handle CORS preflight request. Missing header: {new { Header = HttpHeaders.Origin }}");
				return Task.FromResult(true);
			}

			Logger.LogTrace($"{context.TraceIdentifier} - Try to match CORS policy: {new { Request = context.Request.Description, Origin = origin, Policies = Policies.Select(p => p.Origin).ToReadableArray() }}");

			if (policies.TryGetValue(origin, out var policy))
			{
				var headers = new Headers(policy.AllowMethods, policy.AllowHeaders, policy.Origin, policy.MaxAge);
				context.Response.AddHeader(HttpHeaders.AccessControlAllowMethods, headers.AllowMethods);
				context.Response.AddHeader(HttpHeaders.AccessControlAllowHeaders, headers.AllowHeaders);
				context.Response.AddHeader(HttpHeaders.AccessControlAllowOrigin, headers.Origin);
				context.Response.AddHeader(HttpHeaders.AccessControlMaxAge, headers.MaxAge);
				context.Response.StatusCode = HttpStatusCode.NoContent;
				context.Response.ReadFrom(new MemoryStream());

				Logger.LogTrace($"{context.TraceIdentifier} - Successfully handled CORS preflight request: {new { Policy = policy, AllowMethods = policy.AllowMethods.ToReadableArray(), AllowHeaders = policy.AllowHeaders.ToReadableArray(), Origin = policy.Origin, MaxAge = policy.MaxAge }}");
				return Task.FromResult(true);
			}

			Logger.LogTrace($"{context.TraceIdentifier} - Failed to handle CORS preflight request. Request contains no supported policy: {new { Origin = origin, Policies = Policies.Select(p => p.Origin).ToReadableArray() }}");
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
