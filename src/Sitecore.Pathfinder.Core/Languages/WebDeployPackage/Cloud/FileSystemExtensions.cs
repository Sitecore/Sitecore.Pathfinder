// © 2015-2017 by Jakob Christensen. All rights reserved.

using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Languages.Webdeploy.Cloud
{
    public static class FileSystemExtensions
    {
        public static void AppendFile([NotNull] this IFileSystem fileSystem, [NotNull] string fileName, [NotNull] string content)
        {
            fileSystem.CreateDirectoryFromFileName(fileName);
            File.AppendAllText(fileName, content);
        }

        public static void WriteFile([NotNull] this IFileSystem fileSystem, [NotNull] string fileName, [NotNull] byte[] content)
        {
            var memoryStream = new MemoryStream(content);
            fileSystem.WriteFile(fileName, memoryStream);
        }

        public static void WriteFile([NotNull] this IFileSystem fileSystem, [NotNull] string fileName, [NotNull] string content)
        {
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.WriteAsync(content).GetAwaiter().GetResult();
            writer.FlushAsync().GetAwaiter().GetResult();
            memoryStream.Seek(0, SeekOrigin.Begin);
            fileSystem.WriteFile(fileName, memoryStream);
        }

        public static void WriteFile([NotNull] this IFileSystem fileSystem, [NotNull] string fileName, [NotNull] Stream content)
        {
            fileSystem.CreateDirectoryFromFileName(fileName);

            if (content.Length != 0 && !(fileName.EndsWith(@"\") || fileName.EndsWith("/")))
            {
                using (var fileStream = File.Create(fileName))
                {
                    content.CopyToAsync(fileStream).GetAwaiter().GetResult();
                }
            }

            content.Dispose();
        }
    }
}
