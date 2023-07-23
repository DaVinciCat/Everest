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

			Logger.LogTrace($"{context.Id} - Try to authenticate: {new { Schemes = authentications.Select(kvp => kvp.Value.Scheme).ToReadableArray() }}");

			foreach (var authentication in authentications.Values)
			{
				await authentication.TryAuthenticateAsync(context);
			}
		}
	}
}
