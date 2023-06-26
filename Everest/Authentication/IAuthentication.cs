using Everest.Http;

namespace Everest.Authentication
{
    public interface IAuthentication
	{
		string Scheme { get; }

		bool TryAuthenticate(HttpContext context);
	}
}
