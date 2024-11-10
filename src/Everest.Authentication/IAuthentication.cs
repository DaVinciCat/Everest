using System.Threading.Tasks;
using Everest.Http;
using Everest.Utils;
using Microsoft.Extensions.Logging;

namespace Everest.Authentication
{
    public interface IAuthentication
	{
        Task<bool> TryAuthenticateAsync(IHttpContext context);
	}

    public static partial class HasLoggerExtensions
    {
        public static ILogger GetLogger(this IAuthentication instance) => (instance as IHasLogger)?.Logger;
    }
}
