using System.Threading.Tasks;
using Everest.Http;
using Everest.Utils;
using Microsoft.Extensions.Logging;

namespace Everest.EndPoints
{
    public interface IEndPointInvoker
	{
		Task<bool> TryInvokeEndPointAsync(IHttpContext context);
	}

    public static class HasLoggerExtensions
    {
        public static ILogger GetLogger(this IEndPointInvoker instance) => (instance as IHasLogger)?.Logger;
    }
}
