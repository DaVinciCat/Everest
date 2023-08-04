using System;
using System.Threading.Tasks;
using Everest.Http;
using Everest.Middleware;

namespace Everest.Authentication
{
	public class AuthenticationMiddleware : MiddlewareBase
	{
		private readonly IAuthenticator authenticator;

		public AuthenticationMiddleware(IAuthenticator authenticator)
		{
			this.authenticator = authenticator ?? throw new ArgumentNullException(nameof(authenticator));
		}

		public override async Task InvokeAsync(HttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			if (context.Request.HasAuthentication())
			{
				await authenticator.AuthenticateAsync(context);
			}

			if (HasNext)
				await Next.InvokeAsync(context);
		}
	}
}
