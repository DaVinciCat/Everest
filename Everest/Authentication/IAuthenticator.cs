using System.Collections.Generic;
using Everest.Http;

namespace Everest.Authentication
{
    public interface IAuthenticator
	{
		public IList<IAuthentication> Authentications { get; }

		void Authenticate(HttpContext context);
	}
}
