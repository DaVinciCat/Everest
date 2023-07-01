using System.Collections.Generic;
using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Authentication
{
    public interface IAuthenticator
	{
		public IList<IAuthentication> Authentications { get; }

		Task AuthenticateAsync(HttpContext context);
	}
}
