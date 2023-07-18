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

		public IList<IAuthentication> Authentications { get; set; } = new List<IAuthentication>();
		
		public Authenticator(ILogger<Authenticator> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}
		
		public async Task AuthenticateAsync(HttpContext context)
		{
			if (context == null) 
				throw new ArgumentNullException(nameof(context));

			Logger.LogTrace($"{context.Id} - Try to authenticate: {new { Schemes = Authentications.Select(authentication => authentication.Scheme).ToReadableArray()}}");

			foreach (var authentication in Authentications)
			{
				await authentication.TryAuthenticateAsync(context);
			}
		}
	}
}
