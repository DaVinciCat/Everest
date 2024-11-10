using System;
using System.IO;

namespace Everest.Utils
{
    public static class FileSystemExtensions
    {
        public static void CreateFile(this FileInfo fi)
        {
            if (fi == null)
                throw new ArgumentException(nameof(fi));

            var directory = Path.GetDirectoryName(fi.FullName);

            if (string.IsNullOrWhiteSpace(directory))
                throw new ArgumentException(nameof(directory));

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (File.Create(fi.FullName))
            {

            }
        }

        public static void CreateDirectory(this DirectoryInfo di)
        {
            if (di == null)
                throw new ArgumentException(nameof(di));

            if (!Directory.Exists(di.FullName))
            {
                Directory.CreateDirectory(di.FullName);
            }
        }

        public static void CopyDirectory(this DirectoryInfo di, string destinationDir, bool recursive = true)
        {
            if (di == null)
                throw new ArgumentException(nameof(di));

            var sourceDir = di.FullName;
            var directories = Directory.GetDirectories(di.FullName, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            foreach (var directory in directories)
            {
                var dirToCreate = directory.Replace(sourceDir, destinationDir);
                Directory.CreateDirectory(dirToCreate);
            }

            var files = Directory.GetFiles(sourceDir, "*.*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            foreach (var filePath in files)
            {
                File.Copy(filePath, filePath.Replace(sourceDir, destinationDir));
            }
        }
    }
}
