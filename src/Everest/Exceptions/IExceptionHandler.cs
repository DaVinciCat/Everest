using Everest.Http;
using Everest.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Everest.Exceptions
{
	public interface IExceptionHandler
	{
		Task HandleExceptionAsync(IHttpContext context, Exception ex);
	}

    public static partial class HasLoggerExtensions
    {
        public static ILogger GetLogger(this IExceptionHandler instance) => (instance as IHasLogger)?.Logger;
    }
}
