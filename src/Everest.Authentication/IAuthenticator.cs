using System.Threading.Tasks;
using Everest.Http;
using Everest.Utils;
using Microsoft.Extensions.Logging;

namespace Everest.Authentication
{
    public interface IAuthenticator
	{
		Task<bool> TryAuthenticateAsync(IHttpContext context);

        Task ChallengeAsync(IHttpContext context);
    }

    public static partial class HasLoggerExtensions
    {
        public static ILogger GetLogger(this IAuthenticator instance) => (instance as IHasLogger)?.Logger;
    }
}
