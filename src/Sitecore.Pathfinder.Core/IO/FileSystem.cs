// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO
{
    [Export(typeof(IFileSystem))]
    public class FileSystem : IFileSystem
    {
        public virtual void Copy(string sourceFileName, string destinationFileName, bool forceUpdate = true)
        {
            if (!forceUpdate)
            {
                var fileInfo1 = new FileInfo(sourceFileName);
                var fileInfo2 = new FileInfo(destinationFileName);

                if (fileInfo1.Exists && fileInfo2.Exists && fileInfo1.LastWriteTime.ToUniversalTime() == fileInfo2.LastWriteTime.ToUniversalTime() && fileInfo1.Length == fileInfo2.Length)
                {
                    return;
                }
            }

            CreateDirectoryFromFileName(destinationFileName);
            Copy(sourceFileName, destinationFileName);
            File.SetLastWriteTime(destinationFileName, File.GetLastWriteTime(sourceFileName).ToUniversalTime());
        }

        public bool CopyIfNewer(string sourceFileName, string targetFileName)
        {
            if (!FileExists(targetFileName))
            {
                CreateDirectoryFromFileName(targetFileName);
                Copy(sourceFileName, targetFileName);
                return true;
            }

            if (string.Equals(Path.GetExtension(sourceFileName), ".dll", StringComparison.OrdinalIgnoreCase))
            {
                var sourceVersion = GetVersion(sourceFileName);
                var targetVersion = GetVersion(targetFileName);
                if (targetVersion < sourceVersion)
                {
                    Copy(sourceFileName, targetFileName);
                    return true;
                }
            }

            // update file if length or last write time has changed
            var sourceFileInfo = new FileInfo(sourceFileName);
            var targetFileInfo = new FileInfo(targetFileName);
            if (sourceFileInfo.Length != targetFileInfo.Length || sourceFileInfo.LastWriteTimeUtc > targetFileInfo.LastWriteTimeUtc)
            {
                File.Copy(sourceFileName, targetFileName, true);
                return true;
            }

            return false;
        }

        public virtual void CreateDirectory(string directory)
        {
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public void CreateDirectoryFromFileName(string fileName)
        {
            var directory = Path.GetDirectoryName(fileName);
            if (string.IsNullOrEmpty(directory))
            {
                throw new InvalidOperationException("Cannot create directory from empty filename");
            }

            CreateDirectory(directory);
        }

        public virtual void DeleteDirectory(string directory) => Directory.Delete(directory, true);

        public virtual void DeleteFile(string fileName) => File.Delete(fileName);

        public virtual bool DirectoryExists(string directory) => Directory.Exists(directory);

        public virtual bool FileExists(string fileName) => File.Exists(fileName);

        public virtual IEnumerable<string> GetDirectories(string directory) => Directory.GetDirectories(directory).ToArray();

        public virtual IEnumerable<string> GetFiles(string directory, SearchOption searchOptions = SearchOption.TopDirectoryOnly) => Directory.GetFiles(directory, "*", searchOptions).ToArray();

        public virtual IEnumerable<string> GetFiles(string directory, string pattern, SearchOption searchOptions = SearchOption.TopDirectoryOnly) => Directory.GetFiles(directory, pattern, searchOptions).ToArray();

        public virtual DateTime GetLastWriteTimeUtc(string sourceFileName)
        {
            if (string.IsNullOrEmpty(sourceFileName))
            {
                throw new ArgumentException(Texts.sourceFileName_cannot_be_empty, nameof(sourceFileName));
            }

            return File.GetLastWriteTime(sourceFileName).ToUniversalTime();
        }

        public string GetUniqueFileName(string fileName)
        {
            var result = fileName;
            var index = 0;

            var baseFileName = Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName) + " ";
            var extension = Path.GetExtension(fileName);

            while (FileExists(result))
            {
                result = baseFileName + index + extension;
                index++;
            }

            return result;
        }

        public virtual void Mirror(string sourceDirectory, string destinationDirectory)
        {
            // todo: rewrite for cross platform
            var proc = new Process
            {
                StartInfo =
                {
                    Arguments = $"\"{sourceDirectory.TrimEnd('\\')}\" \"{destinationDirectory.TrimEnd('\\')}\" /mir /njh /njs /ndl /nc /ns /np",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    FileName = "robocopy"
                }
            };

            proc.Start();
            proc.WaitForExit();
        }

        public StreamWriter OpenStreamWriter(string fileName) => new StreamWriter(new FileStream(fileName, FileMode.Create));

        public Stream OpenWrite(string fileName) => new FileStream(fileName, FileMode.Create);

        public virtual string[] ReadAllLines(string fileName) => File.ReadAllLines(fileName);

        public virtual string ReadAllText(string fileName) => File.ReadAllText(fileName);

        public XDocument ReadXml(string fileName, LoadOptions loadOptions = LoadOptions.None)
        {
            var fileInfo = new FileInfo(fileName);
            return XDocument.Load(fileInfo.OpenRead(), loadOptions);
        }

        public void Rename(string oldFileName, string newFileName) => File.Move(oldFileName, newFileName);

        public void Unzip(string zipFileName, string destinationDirectory)
        {
            using (var zip = ZipFile.OpenRead(zipFileName))
            {
                foreach (var entry in zip.Entries)
                {
                    if (entry.FullName.EndsWith("/"))
                    {
                        Directory.CreateDirectory(Path.Combine(destinationDirectory, entry.FullName));
                    }
                    else
                    {
                        var fileName = Path.Combine(destinationDirectory, entry.FullName);
                        CreateDirectoryFromFileName(fileName);
                        entry.ExtractToFile(fileName, true);
                    }
                }
            }
        }

        public virtual void WriteAllBytes(string fileName, byte[] bytes) => File.WriteAllBytes(fileName, bytes);

        public virtual void WriteAllText(string fileName, string contents) => File.WriteAllText(fileName, contents, Encoding.UTF8);

        public virtual void WriteAllText(string fileName, string contents, Encoding encoding) => File.WriteAllText(fileName, contents, encoding);

        public virtual void XCopy(string sourceDirectory, string destinationDirectory)
        {
            // todo: rewrite for cross platform
            var proc = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    FileName = "xcopy.exe",
                    CreateNoWindow = true,
                    Arguments = $"\"{sourceDirectory}\" \"{destinationDirectory}\" /E /I /Y"
                }
            };

            proc.Start();
            proc.WaitForExit();
        }

        [NotNull]
        private Version GetVersion([NotNull] string fileName)
        {
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(fileName);

            try
            {
                return new Version(fileVersionInfo.FileVersion);
            }
            catch
            {
                // silent
            }

            try
            {
                return new Version(fileVersionInfo.ProductVersion);
            }
            catch
            {
                // silent
            }

            return new Version(0, 0, 0, 0);
        }
    }
}
