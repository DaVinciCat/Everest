using System;
using System.IO;

namespace Everest.Utils
{
    public static class FileExtensions
    {
        public static void CreateFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException(nameof(filePath));

            var path = Path.GetDirectoryName(filePath);

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException(nameof(path));

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (File.Create(filePath))
            {

            }
        }
    }
}
