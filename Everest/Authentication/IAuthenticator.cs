using System.Collections.Generic;
using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Authentication
{
    public interface IAuthenticator
	{
		public IAuthentication[] Authentications { get; }

		public void AddAuthentication(IAuthentication authentication);

		Task AuthenticateAsync(HttpContext context);
	}
}
