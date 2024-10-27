using System;
using System.Threading.Tasks;
using Everest.Core.Http;
using Everest.Core.Middlewares;

namespace Everest.Authentication
{
	public class AuthenticationMiddleware : Middleware
	{
		private readonly IAuthenticator authenticator;

		public AuthenticationMiddleware(IAuthenticator authenticator)
		{
			this.authenticator = authenticator ?? throw new ArgumentNullException(nameof(authenticator));
		}

		public override async Task InvokeAsync(IHttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			if (context.Request.SupportsAuthentication())
			{
				await authenticator.TryAuthenticateAsync(context);
			}

			if (HasNext)
				await Next.InvokeAsync(context);
		}
	}
}
