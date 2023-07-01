﻿using System;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Middleware;

namespace Everest.Authentication
{
	public class AuthenticationMiddleware : MiddlewareBase
	{
		private readonly IAuthenticator handler;

		public AuthenticationMiddleware(IAuthenticator handler)
		{
			this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
		}

		public override async Task InvokeAsync(HttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			await handler.AuthenticateAsync(context);

			if (HasNext)
				await Next.InvokeAsync(context);
		}
	}
}
