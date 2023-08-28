using System;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Utils;
using Microsoft.Extensions.Logging;

namespace Everest.Authentication
{
	public class Authenticator : IAuthenticator
	{
        public ILogger<Authenticator> Logger { get; }

        public AuthenticationCollection Authentications { get; } = new();

        public Authenticator(ILogger<Authenticator> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}
		
		public async Task<bool> TryAuthenticateAsync(HttpContext context)
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

            Logger.LogWarning($"{context.TraceIdentifier} - Failed to authenticate. No supported authentication schemes {new { Scheme = scheme, SupportedSchemes = Authentications.ToReadableArray() }}");
            return false;
        }
	}
}
