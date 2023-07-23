using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Authentication
{
    public interface IAuthenticator
	{
		public IAuthentication[] Authentications { get; }

		void AddAuthentication(IAuthentication authentication);

		void RemoveAuthentication(IAuthentication authentication);

		void RemoveAuthentication(string scheme);

		void ClearAuthentications();

		Task AuthenticateAsync(HttpContext context);
	}
}
