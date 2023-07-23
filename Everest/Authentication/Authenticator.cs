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

		public IAuthentication[] Authentications => authentications.ToArray();

		private readonly List<IAuthentication> authentications = new ();

		public Authenticator(ILogger<Authenticator> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public void AddAuthentication(IAuthentication authentication)
		{
			authentications.Add(authentication);
		}

		public async Task AuthenticateAsync(HttpContext context)
		{
			if (context == null) 
				throw new ArgumentNullException(nameof(context));

			Logger.LogTrace($"{context.Id} - Try to authenticate: {new { Schemes = authentications.Select(authentication => authentication.Scheme).ToReadableArray()}}");

			foreach (var authentication in authentications)
			{
				await authentication.TryAuthenticateAsync(context);
			}
		}
	}
}
