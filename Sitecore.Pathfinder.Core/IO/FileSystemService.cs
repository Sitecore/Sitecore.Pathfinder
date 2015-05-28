namespace Sitecore.Pathfinder.IO
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Diagnostics;
  using System.IO;
  using System.Text;
  using Sitecore.Pathfinder.Diagnostics;

  [Export(typeof(IFileSystemService))]
  public class FileSystemService : IFileSystemService
  {
    public virtual void Copy(string sourceFileName, string destinationFileName)
    {
      var directoryName = Path.GetDirectoryName(destinationFileName);
      if (string.IsNullOrEmpty(directoryName))
      {
        throw new DirectoryNotFoundException();
      }

      Directory.CreateDirectory(directoryName);

      File.Copy(sourceFileName, destinationFileName, true);
      File.SetLastWriteTimeUtc(destinationFileName, File.GetLastWriteTimeUtc(sourceFileName));
    }

    public virtual void CreateDirectory(string directory)
    {
      Directory.CreateDirectory(directory);
    }

    public virtual void DeleteDirectory(string directory)
    {
      Directory.Delete(directory, true);
    }

    public virtual void DeleteFile(string fileName)
    {
      File.Delete(fileName);
    }

    public virtual bool DirectoryExists(string directory)
    {
      return Directory.Exists(directory);
    }

    public virtual bool FileExists(string fileName)
    {
      return File.Exists(fileName);
    }

    public virtual IEnumerable<string> GetDirectories(string directory)
    {
      return Directory.GetDirectories(directory);
    }

    public virtual IEnumerable<string> GetFiles([NotNull] string directory, SearchOption searchOptions = SearchOption.TopDirectoryOnly)
    {
      return Directory.GetFiles(directory, "*", searchOptions);
    }

    public virtual IEnumerable<string> GetFiles(string directory, string pattern, SearchOption searchOptions = SearchOption.TopDirectoryOnly)
    {
      return Directory.GetFiles(directory, pattern, searchOptions);
    }

    public virtual DateTime GetLastWriteTimeUtc(string sourceFileName)
    {
      return File.GetLastWriteTimeUtc(sourceFileName);
    }

    public virtual string[] ReadAllLines(string fileName)
    {
      return File.ReadAllLines(fileName);
    }

    public virtual string ReadAllText(string fileName)
    {
      return File.ReadAllText(fileName);
    }

    public virtual void WriteAllText(string fileName, string contents)
    {
      File.WriteAllText(fileName, contents, Encoding.UTF8);
    }

    public virtual void XCopy(string sourceDirectory, string destinationDirectory)
    {
      var proc = new Process
      {
        StartInfo = {
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
  }
}
