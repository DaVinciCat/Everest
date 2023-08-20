using System.IO;

namespace Everest.Files
{
    public interface IStaticFilesProvider
    {
        bool TryGetFile(string filePath, out FileInfo file);
    }
}