using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Authentication
{
    public interface IAuthenticator
	{
		Task<bool> TryAuthenticateAsync(HttpContext context);

        Task ChallengeAsync(HttpContext context);
    }
}
