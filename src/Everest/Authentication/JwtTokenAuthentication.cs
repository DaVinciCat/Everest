using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Security;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Everest.Authentication
{
	public class JwtAuthentication : IAuthentication
	{
		public ILogger<JwtAuthentication> Logger { get; }

		public string Scheme { get; set; } = "Bearer";

		public TokenValidationParameters TokenValidationParameters { get; set; } = new TokenValidationParameters();

		public JwtAuthentication(ILogger<JwtAuthentication> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<bool> TryAuthenticateAsync(HttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			var header = context.Request.Headers[HttpHeaders.Authorization];
			if (string.IsNullOrWhiteSpace(header))
			{
                if (Logger.IsEnabled(LogLevel.Warning))
                    Logger.LogWarning($"{context.TraceIdentifier} - Failed to authenticate. Missing header: {new { Header = HttpHeaders.Authorization, Scheme = Scheme }}");
				
                return false;
			}

			if (Scheme == HttpHeaders.Authorization)
			{
                if (Logger.IsEnabled(LogLevel.Warning))
                    Logger.LogWarning($"{context.TraceIdentifier} - Failed to authenticate. No token supplied: {new { Scheme = Scheme }}");
				
                return false;
			}

			if (!header.StartsWith(Scheme + ' ', StringComparison.OrdinalIgnoreCase))
			{
                if (Logger.IsEnabled(LogLevel.Warning))
                    Logger.LogWarning($"{context.TraceIdentifier} - Failed to authenticate. Incorrect header: {new { Header = HttpHeaders.Authorization, Scheme = Scheme }}");
				
                return false;
			}

			try
			{
				var token = header.Substring(Scheme.Length).Trim();
				var tokenHandler = new JwtSecurityTokenHandler();
				var validationResult = await tokenHandler.ValidateTokenAsync(token, TokenValidationParameters);
				var jwtToken = validationResult.SecurityToken as JwtSecurityToken;
				var identity = new JwtTokenIdentity(jwtToken, validationResult.ClaimsIdentity);
				context.User.AddIdentity(identity);

                if (Logger.IsEnabled(LogLevel.Trace))
                    Logger.LogTrace($"{context.TraceIdentifier} - Successfully authenticated: {new { Scheme = Scheme }}");

				return true;
			}
			catch (Exception ex)
			{
                if (Logger.IsEnabled(LogLevel.Error))
                    Logger.LogError(ex, $"{context.TraceIdentifier} - Failed to authenticate. Failed to validate token: {new { Scheme = Scheme }}");

				return false;
			}
		}
	}
}
