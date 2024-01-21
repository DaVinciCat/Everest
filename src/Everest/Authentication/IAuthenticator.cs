using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Authentication
{
    public interface IAuthenticator
	{
		Task<bool> TryAuthenticateAsync(IHttpContext context);

        Task ChallengeAsync(IHttpContext context);
    }
}
