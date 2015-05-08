namespace Sitecore.Pathfinder.Building.Preprocessing.IncrementalBuilds.Preprocessors
{
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Pathfinder.IO;

  [Export(typeof(IPreprocessor))]
  public class MediaFilePreprocessor : PreprocessorBase
  {
    public MediaFilePreprocessor() : base("media-file")
    {
    }

    public override void Execute(IPreprocessingContext context, string fileName)
    {
      var serializationDirectory = Path.Combine(context.BuildContext.OutputDirectory, context.SerializationDirectory);
      var destinationFileName = Path.Combine(serializationDirectory, Path.GetFileName(fileName));

      if (PathHelper.CompareFiles(fileName, destinationFileName))
      {
        return;
      }

      context.BuildContext.FileSystem.Copy(fileName, destinationFileName);

      context.BuildContext.ModifiedProjectItems.Add(destinationFileName);
      context.BuildContext.SourceMap.Add(destinationFileName, fileName);
    }
  }
}
