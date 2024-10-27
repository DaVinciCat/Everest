using System.Threading.Tasks;
using Everest.Common.Logging;
using Everest.Core.Http;
using Microsoft.Extensions.Logging;

namespace Everest.Cors
{
	public interface ICorsRequestHandler
	{
		Task<bool> TryHandleCorsRequestAsync(IHttpContext context);
	}

    public static class HasLoggerExtensions
    {
        public static ILogger GetLogger(this ICorsRequestHandler instance) => (instance as IHasLogger)?.Logger;
    }
}
