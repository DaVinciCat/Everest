using Everest.Utils;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Everest.Files
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