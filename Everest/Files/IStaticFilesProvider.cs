using System.IO;

namespace Everest.Files
{
    public interface IStaticFilesProvider
    {
        public string PhysicalPath { get; }

        bool TryGetFile(string filePath, out FileInfo file);
    }
}