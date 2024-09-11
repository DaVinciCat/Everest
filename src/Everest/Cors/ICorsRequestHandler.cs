using System.Threading.Tasks;
using Everest.Http;
using Everest.Utils;
using Microsoft.Extensions.Logging;

namespace Everest.Cors
{
	public interface ICorsRequestHandler
	{
		Task<bool> TryHandleCorsRequestAsync(IHttpContext context);
	}

    public static partial class HasLoggerExtensions
    {
        public static ILogger GetLogger(this ICorsRequestHandler instance) => (instance as IHasLogger)?.Logger;
    }
}
