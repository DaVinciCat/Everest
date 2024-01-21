using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Authentication
{
    public interface IAuthentication
	{
        Task<bool> TryAuthenticateAsync(IHttpContext context);
	}
}
