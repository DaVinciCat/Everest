using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Everest.Common.Logging;
using Everest.Core.Http;

namespace Everest.Exceptions
{
	public interface IExceptionHandler
	{
		Task HandleExceptionAsync(IHttpContext context, Exception ex);
	}

    public static class HasLoggerExtensions
    {
        public static ILogger GetLogger(this IExceptionHandler instance) => (instance as IHasLogger)?.Logger;
    }
}
