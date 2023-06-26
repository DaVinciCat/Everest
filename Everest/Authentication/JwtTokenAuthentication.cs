using System;
using System.IdentityModel.Tokens.Jwt;
using Everest.Http;
using Everest.Security;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Everest.Authentication
{
	public class JwtAuthenticationOptions
	{
		public string Scheme { get; set; } = "Bearer";

		public string Header { get; set; } = "Authorization";

		public TokenValidationParameters TokenValidationParameters { get; set; }

		public JwtAuthenticationOptions()
		{
			TokenValidationParameters = new TokenValidationParameters();
		}
	}

    public class JwtAuthentication : IAuthentication
	{
		public ILogger<JwtAuthentication> Logger { get; }

		public string Scheme => options.Scheme;

		private readonly JwtAuthenticationOptions options;

		public JwtAuthentication(JwtAuthenticationOptions options, ILogger<JwtAuthentication> logger)
		{
			this.options = options ?? throw new ArgumentNullException(nameof(options));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public JwtAuthentication(ILogger<JwtAuthentication> logger)
			: this(new JwtAuthenticationOptions(), logger)
		{
			
		}

		public bool TryAuthenticate(HttpContext context)
		{
			if (context == null) 
				throw new ArgumentNullException(nameof(context));

			var header = context.Request.Headers[options.Header];
			if (string.IsNullOrEmpty(header))
			{
				Logger.LogWarning($"{context.Id} - Failed to authenticate. Missing header: {options.Header}. Scheme: {Scheme}. ");
				return false;
			}

			if (Scheme == options.Header)
			{
				Logger.LogWarning($"{context.Id} - Failed to authenticate. No token supplied. Scheme: {Scheme}");
				return false;
			}

			if (!header.StartsWith(Scheme + ' ', StringComparison.OrdinalIgnoreCase))
			{
				Logger.LogWarning($"{context.Id} - Failed to authenticate. Wrong header: {options.Header}. Scheme: {Scheme}");
				return false;
			}

			try
			{
				var token = header.Substring(Scheme.Length).Trim();
				var tokenHandler = new JwtSecurityTokenHandler();
				var claimsPrincipal = tokenHandler.ValidateToken(token, options.TokenValidationParameters, out var validatedToken);
				var jwtToken = validatedToken as JwtSecurityToken;
				var identity = new JwtTokenIdentity(jwtToken, claimsPrincipal.Identity);
				context.User.AddIdentity(identity);

				Logger.LogTrace($"{context.Id} - Successfully authenticated. Scheme: {Scheme}");
				return true;
			}
			catch(Exception ex)
			{
				Logger.LogError(ex, $"{context.Id} - Failed to authenticate. Failed to validate token. Scheme: {Scheme}");
				return false;
			}
		}
	}
}
