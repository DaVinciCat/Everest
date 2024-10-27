using System.IO;
using Everest.Common.Logging;
using Microsoft.Extensions.Logging;

namespace Everest.StaticFiles
{
    public interface IStaticFilesProvider
    {
        bool TryGetFile(string filePath, out FileInfo file);
    }

    public static partial class HasLoggerExtensions
    {
        public static ILogger GetLogger(this IStaticFilesProvider instance) => (instance as IHasLogger)?.Logger;
    }
}