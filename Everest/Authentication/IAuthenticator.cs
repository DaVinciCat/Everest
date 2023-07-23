using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Authentication
{
    public interface IAuthenticator
	{
		public IAuthentication[] Authentications { get; }

		void AddAuthentication(IAuthentication authentication);

		Task AuthenticateAsync(HttpContext context);
	}
}
