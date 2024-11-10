using System;
using Microsoft.Extensions.Logging;

namespace Everest.Utils
{
    #region Logging

    public static class LoggerExtensions
    {
        public static void LogDebugIfEnabled(this ILogger logger, Func<(Exception Exception, string Message, object[] Args)> func)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                var @params = func();
                logger.LogDebug(@params.Exception, @params.Message, @params.Args);
            }
        }

        public static void LogDebugIfEnabled(this ILogger logger, Func<(Exception Exception, string Message)> func)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                var @params = func();
                logger.LogDebug(@params.Exception, @params.Message);
            }
        }

        public static void LogDebugIfEnabled(this ILogger logger, Func<(string Message, object[] Args)> func)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                var @params = func();
                logger.LogDebug(@params.Message, @params.Args);
            }
        }

        public static void LogDebugIfEnabled(this ILogger logger, Func<string> func)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                var @params = func();
                logger.LogDebug(@params);
            }
        }

        public static void LogTraceIfEnabled(this ILogger logger, Func<(Exception Exception, string Message, object[] Args)> func)
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                var @params = func();
                logger.LogTrace(@params.Exception, @params.Message, @params.Args);
            }
        }

        public static void LogTraceIfEnabled(this ILogger logger, Func<(Exception Exception, string Message)> func)
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                var @params = func();
                logger.LogTrace(@params.Exception, @params.Message);
            }
        }

        public static void LogTraceIfEnabled(this ILogger logger, Func<(string Message, object[] Args)> func)
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                var @params = func();
                logger.LogTrace(@params.Message, @params.Args);
            }
        }

        public static void LogTraceIfEnabled(this ILogger logger, Func<string> func)
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                var @params = func();
                logger.LogTrace(@params);
            }
        }

        public static void LogInformationIfEnabled(this ILogger logger, Func<(Exception Exception, string Message, object[] Args)> func)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                var @params = func();
                logger.LogInformation(@params.Exception, @params.Message, @params.Args);
            }
        }

        public static void LogInformationIfEnabled(this ILogger logger, Func<(Exception Exception, string Message)> func)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                var @params = func();
                logger.LogInformation(@params.Exception, @params.Message);
            }
        }

        public static void LogInformationIfEnabled(this ILogger logger, Func<(string Message, object[] Args)> func)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                var @params = func();
                logger.LogInformation(@params.Message, @params.Args);
            }
        }

        public static void LogInformationIfEnabled(this ILogger logger, Func<string> func)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                var @params = func();
                logger.LogInformation(@params);
            }
        }

        public static void LogWarningIfEnabled(this ILogger logger, Func<(Exception Exception, string Message, object[] Args)> func)
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                var @params = func();
                logger.LogWarning(@params.Exception, @params.Message, @params.Args);
            }
        }

        public static void LogWarningIfEnabled(this ILogger logger, Func<(Exception Exception, string Message)> func)
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                var @params = func();
                logger.LogWarning(@params.Exception, @params.Message);
            }
        }

        public static void LogWarningIfEnabled(this ILogger logger, Func<(string Message, object[] Args)> func)
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                var @params = func();
                logger.LogWarning(@params.Message, @params.Args);
            }
        }

        public static void LogWarningIfEnabled(this ILogger logger, Func<string> func)
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                var @params = func();
                logger.LogWarning(@params);
            }
        }

        public static void LogErrorIfEnabled(this ILogger logger, Func<(Exception Exception, string Message, object[] Args)> func)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                var @params = func();
                logger.LogError(@params.Exception, @params.Message, @params.Args);
            }
        }

        public static void LogErrorIfEnabled(this ILogger logger, Func<(Exception Exception, string Message)> func)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                var @params = func();
                logger.LogError(@params.Exception, @params.Message);
            }
        }

        public static void LogErrorIfEnabled(this ILogger logger, Func<(string Message, object[] Args)> func)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                var @params = func();
                logger.LogError(@params.Message, @params.Args);
            }
        }

        public static void LogErrorIfEnabled(this ILogger logger, Func<string> func)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                var @params = func();
                logger.LogError(@params);
            }
        }

        public static void LogCriticalIfEnabled(this ILogger logger, Func<(Exception Exception, string Message, object[] Args)> func)
        {
            if (logger.IsEnabled(LogLevel.Critical))
            {
                var @params = func();
                logger.LogCritical(@params.Exception, @params.Message, @params.Args);
            }
        }

        public static void LogCriticalIfEnabled(this ILogger logger, Func<(Exception Exception, string Message)> func)
        {
            if (logger.IsEnabled(LogLevel.Critical))
            {
                var @params = func();
                logger.LogCritical(@params.Exception, @params.Message);
            }
        }

        public static void LogCriticalIfEnabled(this ILogger logger, Func<(string Message, object[] Args)> func)
        {
            if (logger.IsEnabled(LogLevel.Critical))
            {
                var @params = func();
                logger.LogCritical(@params.Message, @params.Args);
            }
        }

        public static void LogCriticalIfEnabled(this ILogger logger, Func<string> func)
        {
            if (logger.IsEnabled(LogLevel.Critical))
            {
                var @params = func();
                logger.LogCritical(@params);
            }
        }


        public static void LogIfEnabled(this ILogger logger, LogLevel logLevel, Func<(Exception Exception, string Message, object[] Args)> func)
        {
            if (logger.IsEnabled(logLevel))
            {
                var @params = func();
                logger.Log(logLevel, @params.Exception, @params.Message, @params.Args);
            }
        }

        public static void LogIfEnabled(this ILogger logger, LogLevel logLevel, Func<(Exception Exception, string Message)> func)
        {
            if (logger.IsEnabled(logLevel))
            {
                var @params = func();
                logger.Log(logLevel, @params.Exception, @params.Message);
            }
        }

        public static void LogIfEnabled(this ILogger logger, LogLevel logLevel, Func<(string Message, object[] Args)> func)
        {
            if (logger.IsEnabled(logLevel))
            {
                var @params = func();
                logger.Log(logLevel, @params.Message, @params.Args);
            }
        }

        public static void LogIfEnabled(this ILogger logger, LogLevel logLevel, Func<string> func)
        {
            if (logger.IsEnabled(logLevel))
            {
                var @params = func();
                logger.Log(logLevel, @params);
            }
        }
    }

    #endregion

    #region IHasLogger

    public interface IHasLogger
    {
        ILogger Logger { get; }
    }

    #endregion
}