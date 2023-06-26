using System;
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

        public override void Invoke(HttpContext context)
        {
	        if (context == null) 
		        throw new ArgumentNullException(nameof(context));

	        handler.Authenticate(context);

            if (HasNext)
                Next.Invoke(context);
        }
    }
}
