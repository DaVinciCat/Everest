using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Utils;
using Microsoft.Extensions.Logging;

namespace Everest.Authentication
{
	public class Authenticator : IAuthenticator
	{
		public ILogger<Authenticator> Logger { get; }

		public IAuthentication[] Authentications => authentications.Values.ToArray();

		private readonly Dictionary<string, IAuthentication> authentications = new();

		public Authenticator(ILogger<Authenticator> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public void AddAuthentication(IAuthentication authentication)
		{
			authentications[authentication.Scheme] = authentication;
		}

		public void RemoveAuthentication(IAuthentication authentication)
		{
			if (authentications.ContainsKey(authentication.Scheme))
			{
				authentications.Remove(authentication.Scheme);
			}
		}

		public void RemoveAuthentication(string scheme)
		{
			if (authentications.ContainsKey(scheme))
			{
				authentications.Remove(scheme);
			}
		}

		public void ClearAuthentications()
		{
			authentications.Clear();
		}

		public async Task AuthenticateAsync(HttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));
			
			if (!authentications.Any(authentication => context.Request.SupportsAuthenticationScheme(authentication.Value.Scheme)))
			{
				context.Request.TryGetAuthenticationScheme(out var scheme);
				Logger.LogWarning($"{context.TraceIdentifier} - Failed to authenticate. No supported authentication schemes {new {Scheme = scheme, SupportedSchemes = authentications.Keys.ToReadableArray()}}");
			}

			foreach (var authentication in authentications.Values)
			{
				if(context.Request.SupportsAuthenticationScheme(authentication.Scheme))
				{
					await authentication.TryAuthenticateAsync(context);
				}
			}
		}
	}
}
