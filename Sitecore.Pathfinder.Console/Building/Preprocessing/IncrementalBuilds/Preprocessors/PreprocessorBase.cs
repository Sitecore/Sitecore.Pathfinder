namespace Sitecore.Pathfinder.Building.Preprocessing.IncrementalBuilds.Preprocessors
{
  using System.IO;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.Pathfinder.IO;

  public abstract class PreprocessorBase : IPreprocessor
  {
    protected PreprocessorBase([NotNull] string name)
    {
      this.Name = name;
    }

    public string Name { get; }

    public void Copy([NotNull] IPreprocessingContext context, [NotNull] string sourceFileName, [NotNull] string destinationFileName)
    {
      var destinationDirectoryName = Path.GetDirectoryName(destinationFileName);
      if (string.IsNullOrEmpty(destinationDirectoryName))
      {
        throw new BuildException(ConsoleTexts.Text3010);
      }

      context.BuildContext.FileSystem.CreateDirectory(destinationDirectoryName);

      var fileName = Path.GetFileName(destinationFileName);
      var fileNameWithoutExtensions = PathHelper.GetFileNameWithoutExtensions(destinationFileName);
      var contentDirectoryName = PathHelper.NormalizeWebPath(context.ContentDirectory).Mid("content".Length);

      var contents = context.BuildContext.FileSystem.ReadAllText(sourceFileName);

      contents = contents.Replace("$ItemPath", context.ItemPath);
      contents = contents.Replace("$Database", context.Database);
      contents = contents.Replace("$FileNameWithoutExtensions", fileNameWithoutExtensions);
      contents = contents.Replace("$FileName", fileName);
      contents = contents.Replace("$ContentDirectoryName", contentDirectoryName);

      context.BuildContext.FileSystem.WriteAllText(destinationFileName, contents);

      File.SetLastWriteTimeUtc(destinationFileName, File.GetLastWriteTimeUtc(sourceFileName));
    }

    public abstract void Execute(IPreprocessingContext context, string fileName);

    protected virtual bool HasItemFile([NotNull] IPreprocessingContext context, [NotNull] string fileName)
    {
      var baseFileName = PathHelper.GetDirectoryAndFileNameWithoutExtensions(fileName);

      // todo: make this configurable
      if (context.BuildContext.FileSystem.FileExists(baseFileName + ".item.xml"))
      {
        return true;
      }

      if (context.BuildContext.FileSystem.FileExists(baseFileName + ".item"))
      {
        return true;
      }

      return false;
    }
  }
}
