﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Everest.Http;
using Microsoft.Extensions.Logging;

namespace Everest.Authentication
{
	public class Authenticator : IAuthenticator
	{
		public ILogger<Authenticator> Logger { get; }

		public IList<IAuthentication> Authentications { get; } = new List<IAuthentication>();
		
		public Authenticator(ILogger<Authenticator> logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}
		
		public async Task AuthenticateAsync(HttpContext context)
		{
			if (context == null) 
				throw new ArgumentNullException(nameof(context));

			Logger.LogTrace($"{context.Id} - Try authenticate. Schemes: [{string.Join(", ", Authentications.Select(authentication => authentication.Scheme))}]");

			foreach (var authentication in Authentications)
			{
				await authentication.TryAuthenticateAsync(context);
			}
		}
	}
}
