﻿using System;
using System.Net;
using Everest.Http;
using Microsoft.Extensions.Logging;

namespace Everest.Cors
{
	/*
	   https://developer.mozilla.org/en-US/docs/Glossary/Preflight_request
    */

	public class CorsPolicy
	{
		public static CorsPolicy Default { get; } = new();

		public string Origin { get; set; } = "*";

		public string[] AllowMethods { get; set; } = { "GET", "POST", "PUT", "DELETE" };

		public string[] AllowHeaders { get; set; } = { "Content-Type", "Accept", "X-Requested-With" };
		
		public int MaxAge { get; set; } = 1728000;

		public CorsPolicy(string origin, string[] allowMethods, string[] allowHeaders, int maxAge)
		{
			Origin = origin;
			AllowMethods = allowMethods;
			AllowHeaders = allowHeaders;
			MaxAge = maxAge;
		}

		private CorsPolicy()
		{

		}
	}
	
	public class CorsRequestHandler : ICorsRequestHandler
	{
		public ILogger<CorsRequestHandler> Logger { get; }

		public CorsPolicyCollection Policies { get; set; } = new();
		
		public CorsRequestHandler(ILogger<CorsRequestHandler> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}
		
		public bool TryHandleCorsRequest(HttpContext context)
		{
			if (context == null) 
				throw new ArgumentNullException(nameof(context));

			if (!context.Request.IsCorsPreflight())
			{
				return false;
			}

			var origin = context.Request.Headers["Origin"];
			Logger.LogTrace($"{context.Id} - Try handle CORS preflight request: {context.Request.Description}. Origin: {origin}");
			
			foreach (var policy in Policies)
			{
				if (string.Equals(policy.Origin, origin, StringComparison.Ordinal))
				{
					var headers = new Headers(policy.AllowMethods, policy.AllowHeaders, policy.Origin, policy.MaxAge);
					context.Response.AddHeader("Access-Control-Allow-Methods", headers.AllowMethods);
					context.Response.AddHeader("Access-Control-Allow-Headers", headers.AllowHeaders);
					context.Response.AddHeader("Access-Control-Allow-Origin", headers.Origin);
					context.Response.AddHeader("Access-Control-Max-Age", headers.MaxAge);
					context.Response.StatusCode = HttpStatusCode.NoContent;

					Logger.LogTrace($"{context.Id} - Successfully handled CORS preflight request");
					return true;
				}
			}

			Logger.LogWarning($"{context.Id} - Failed to handle CORS preflight request. Request contains no supported Origin: {origin}");
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
