// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using Sitecore.Pathfinder.Diagnostics;
using ZetaLongPaths;
using FileHelper = ZetaLongPaths.ZlpIOHelper;

namespace Sitecore.Pathfinder.IO
{
    [Export(typeof(IFileSystemService))]
    public class FileSystemService : IFileSystemService
    {
        [ImportingConstructor]
        public FileSystemService([NotNull] IConsoleService console)
        {
            Console = console;
        }

        [NotNull]
        protected IConsoleService Console { get; }

        public virtual bool CanWriteDirectory(string directory)
        {
            try
            {
                Directory.GetAccessControl(directory);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        public virtual void Copy(string sourceFileName, string destinationFileName, bool forceUpdate = true)
        {
            if (!forceUpdate)
            {
                // var fileInfo1 = new ZlpFileInfo(sourceFileName);
                // var fileInfo2 = new ZlpFileInfo(destinationFileName);
                var fileInfo1 = new FileInfo(sourceFileName);
                var fileInfo2 = new FileInfo(destinationFileName);

                if (fileInfo1.Exists && fileInfo2.Exists && fileInfo1.LastWriteTime.ToUniversalTime() == fileInfo2.LastWriteTime.ToUniversalTime() && fileInfo1.Length == fileInfo2.Length)
                {
                    return;
                }
            }

            var directoryName = Path.GetDirectoryName(destinationFileName);
            if (string.IsNullOrEmpty(directoryName))
            {
                throw new DirectoryNotFoundException();
            }

            FileHelper.CreateDirectory(directoryName);

            FileHelper.CopyFile(sourceFileName, destinationFileName, true);
            FileHelper.SetFileLastWriteTime(destinationFileName, FileHelper.GetFileLastWriteTime(sourceFileName).ToUniversalTime());
        }

        public bool CopyIfNewer(string sourceFileName, string targetFileName)
        {
            if (!FileExists(targetFileName))
            {
                CreateDirectoryFromFileName(targetFileName);
                FileHelper.CopyFile(sourceFileName, targetFileName, true);
                return true;
            }

            if (string.Equals(Path.GetExtension(sourceFileName), ".dll", StringComparison.OrdinalIgnoreCase))
            {
                var sourceVersion = GetVersion(sourceFileName);
                var targetVersion = GetVersion(targetFileName);
                if (targetVersion < sourceVersion)
                {
                    FileHelper.CopyFile(sourceFileName, targetFileName, true);
                    return true;
                }
            }

            // update file if length or last write time has changed
            var sourceFileInfo = new FileInfo(sourceFileName);
            var targetFileInfo = new FileInfo(targetFileName);
            if (sourceFileInfo.Length != targetFileInfo.Length || sourceFileInfo.LastWriteTimeUtc > targetFileInfo.LastWriteTimeUtc)
            {
                FileHelper.CopyFile(sourceFileName, targetFileName, true);
                return true;
            }

            return false;
        }

        public virtual void CreateDirectory(string directory)
        {
            if (!string.IsNullOrEmpty(directory))
            {
                FileHelper.CreateDirectory(directory);
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

        public virtual void DeleteDirectory(string directory)
        {
            FileHelper.DeleteDirectory(directory, true);
        }

        public virtual void DeleteFile(string fileName)
        {
            FileHelper.DeleteFile(fileName);
        }

        public object Deserialize(string fileName, Type type)
        {
            var serializer = new XmlSerializer(type);
            using (var stream = OpenRead(fileName))
            {
                return serializer.Deserialize(stream);
            }
        }

        public virtual bool DirectoryExists(string directory)
        {
            return FileHelper.DirectoryExists(directory);
        }

        public virtual bool FileExists(string fileName)
        {
            return FileHelper.FileExists(fileName);
        }

        public bool FileExistsInPath(string fileName)
        {
            if (FileExists(fileName))
            {
                return true;
            }

            var paths = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            foreach (var path in paths.Split(';'))
            {
                var fullPath = Path.Combine(path.Trim(), fileName);

                if (FileExists(fullPath))
                {
                    return true;
                }
            }

            return false;
        }

        public virtual IEnumerable<string> GetDirectories(string directory)
        {
            return FileHelper.GetDirectories(directory).Select(d => d.FullName).ToArray();
        }

        public virtual IEnumerable<string> GetFiles(string directory, SearchOption searchOptions = SearchOption.TopDirectoryOnly)
        {
            return FileHelper.GetFiles(directory, "*", searchOptions).Select(f => f.FullName).ToArray();
        }

        public virtual IEnumerable<string> GetFiles(string directory, string pattern, SearchOption searchOptions = SearchOption.TopDirectoryOnly)
        {
            return FileHelper.GetFiles(directory, pattern, searchOptions).Select(d => d.FullName).ToArray();
        }

        public virtual DateTime GetLastWriteTimeUtc(string sourceFileName)
        {
            if (string.IsNullOrEmpty(sourceFileName))
            {
                throw new ArgumentException(Texts.sourceFileName_cannot_be_empty, nameof(sourceFileName));
            }

            return FileHelper.GetFileLastWriteTime(sourceFileName).ToUniversalTime();
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
            var proc = new Process
            {
                StartInfo =
                {
                    Arguments = $"\"{sourceDirectory.TrimEnd('\\')}\" \"{destinationDirectory.TrimEnd('\\')}\" /mir /njh /njs /ndl /nc /ns /np",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "robocopy"
                }
            };

            proc.Start();
            proc.WaitForExit();
        }

        public Stream OpenRead(string fileName)
        {
            var fileInfo = new ZlpFileInfo(fileName);
            return fileInfo.OpenRead();
        }

        public StreamReader OpenStreamReader(string fileName)
        {
            var fileInfo = new ZlpFileInfo(fileName);
            return new StreamReader(fileInfo.OpenRead());
        }

        public StreamWriter OpenStreamWriter(string fileName)
        {
            // todo: there is a weird bug in ZetaLongPath that does not truncate the file
            var fileInfo = new FileInfo(fileName);
            return new StreamWriter(fileInfo.OpenWrite());
        }

        public Stream OpenWrite(string fileName)
        {
            var fileInfo = new ZlpFileInfo(fileName);
            return fileInfo.OpenWrite();
        }

        public virtual string[] ReadAllLines(string fileName)
        {
            return FileHelper.ReadAllLines(fileName);
        }

        public virtual string ReadAllText(string fileName)
        {
            return FileHelper.ReadAllText(fileName);
        }

        public XDocument ReadXml(string fileName, LoadOptions loadOptions = LoadOptions.None)
        {
            var fileInfo = new ZlpFileInfo(fileName);
            return XDocument.Load(fileInfo.OpenRead(), loadOptions);
        }

        public void Rename(string oldFileName, string newFileName)
        {
            FileHelper.MoveFile(oldFileName, newFileName);
        }

        public void Serialize(string fileName, Type type, object value)
        {
            // do not use ZetaLongPath - has weird bug
            var serializer = new XmlSerializer(type);
            using (var stream = new StreamWriter(fileName))
            {
                serializer.Serialize(stream, value);
            }
        }

        public void Unzip(string zipFileName, string destinationDirectory)
        {
            using (var zip = ZipFile.OpenRead(zipFileName))
            {
                foreach (var entry in zip.Entries)
                {
                    try
                    {
                        if (entry.FullName.EndsWith("/"))
                        {
                            FileHelper.CreateDirectory(Path.Combine(destinationDirectory, entry.FullName));
                        }
                        else
                        {
                            var fileName = Path.Combine(destinationDirectory, entry.FullName);
                            CreateDirectoryFromFileName(fileName);
                            entry.ExtractToFile(fileName, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        public virtual void WriteAllBytes(string fileName, byte[] bytes)
        {
            FileHelper.WriteAllBytes(fileName, bytes);
        }

        public virtual void WriteAllText(string fileName, string contents)
        {
            FileHelper.WriteAllText(fileName, contents, Encoding.UTF8);
        }

        public virtual void WriteAllText(string fileName, string contents, Encoding encoding)
        {
            FileHelper.WriteAllText(fileName, contents, encoding);
        }

        public virtual void XCopy(string sourceDirectory, string destinationDirectory)
        {
            var proc = new Process
            {
                StartInfo =
                {
                    UseShellExecute = true,
                    FileName = "xcopy.exe",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
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
