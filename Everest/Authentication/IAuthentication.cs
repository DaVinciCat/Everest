using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Authentication
{
    public interface IAuthentication
	{
		string Scheme { get; }

		Task<bool> TryAuthenticateAsync(HttpContext context);
	}
}
