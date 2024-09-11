using System;
using System.Net;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Utils;
using Microsoft.Extensions.Logging;

namespace Everest.Authentication
{
	public class Authenticator : IAuthenticator, IHasLogger
	{
        ILogger IHasLogger.Logger => Logger;

        public ILogger<Authenticator> Logger { get; }

        public AuthenticationCollection Authentications { get; } = new AuthenticationCollection();
		
        public Authenticator(ILogger<Authenticator> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}
		
		public async Task<bool> TryAuthenticateAsync(IHttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			if (context.Request.TryGetAuthenticationScheme(out var scheme))
			{
				if (Authentications.TryGet(scheme, out var authentication))
				{
					return await authentication.TryAuthenticateAsync(context);
                }
			}

            Logger.LogWarningIfEnabled(() => $"{context.TraceIdentifier} - Failed to authenticate. No supported authentication schemes {new { Scheme = scheme, SupportedSchemes = Authentications.ToReadableArray() }}");
            
            return false;
        }

        public Task ChallengeAsync(IHttpContext context)
        {
            context.Response.StatusCode = HttpStatusCode.Unauthorized;

            foreach (var scheme in Authentications)
            {
                context.Response.Headers.Add(HttpHeaders.WwwAuthenticate, scheme);
            }

			return Task.CompletedTask;
        }
    }
}
