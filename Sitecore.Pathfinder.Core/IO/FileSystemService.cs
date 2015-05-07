namespace Sitecore.Pathfinder.IO
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Diagnostics;
  using System.IO;
  using System.Text;
  using Sitecore.Pathfinder.Diagnostics;

  [Export(typeof(IFileSystemService))]
  public class FileSystemService : IFileSystemService
  {
    [ImportingConstructor]
    public FileSystemService([NotNull] ITraceService trace)
    {
      this.Trace = trace;
    }

    [NotNull]
    public ITraceService Trace { get; }

    public void Copy(string sourceFileName, string destinationFileName)
    {
      var directoryName = Path.GetDirectoryName(destinationFileName);
      if (string.IsNullOrEmpty(directoryName))
      {
        this.Trace.TraceError(Texts.Text2028);
        return;
      }

      Directory.CreateDirectory(directoryName);

      File.Copy(sourceFileName, destinationFileName, true);
      File.SetLastWriteTimeUtc(destinationFileName, File.GetLastWriteTimeUtc(sourceFileName));
    }

    public void CreateDirectory(string directory)
    {
      Directory.CreateDirectory(directory);
    }

    public void DeleteDirectory(string directory)
    {
      Directory.Delete(directory, true);
    }

    public void DeleteFile(string fileName)
    {
      File.Delete(fileName);
    }

    public bool DirectoryExists(string directory)
    {
      return Directory.Exists(directory);
    }

    public bool FileExists(string fileName)
    {
      return File.Exists(fileName);
    }

    public IEnumerable<string> GetDirectories(string directory)
    {
      return Directory.GetDirectories(directory);
    }

    public IEnumerable<string> GetFiles(string directory)
    {
      return Directory.GetFiles(directory);
    }

    public IEnumerable<string> GetFiles(string directory, string pattern)
    {
      return Directory.GetFiles(directory, pattern);
    }

    public string[] ReadAllLines(string fileName)
    {
      return File.ReadAllLines(fileName);
    }

    public string ReadAllText(string fileName)
    {
      return File.ReadAllText(fileName);
    }

    public void WriteAllText(string fileName, string contents)
    {
      File.WriteAllText(fileName, contents, Encoding.UTF8);
    }

    public void XCopy(string sourceDirectory, string destinationDirectory)
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
