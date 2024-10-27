using System.Threading.Tasks;
using Everest.Common.Logging;
using Everest.Core.Http;
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
