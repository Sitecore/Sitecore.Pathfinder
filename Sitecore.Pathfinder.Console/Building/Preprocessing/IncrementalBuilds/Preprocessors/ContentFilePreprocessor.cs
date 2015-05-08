namespace Sitecore.Pathfinder.Building.Preprocessing.IncrementalBuilds.Preprocessors
{
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Pathfinder.IO;

  [Export(typeof(IPreprocessor))]
  public class ContentFilePreprocessor : PreprocessorBase
  {
    public ContentFilePreprocessor() : base("content-file")
    {
    }

    public override void Execute(IPreprocessingContext context, string fileName)
    {
      var contentDirectory = Path.Combine(context.BuildContext.OutputDirectory, context.ContentDirectory);
      var destinationFileName = Path.Combine(contentDirectory, Path.GetFileName(fileName));

      if (PathHelper.CompareFiles(fileName, destinationFileName))
      {
        return;
      }

      this.Copy(context, fileName, destinationFileName);

      context.BuildContext.ModifiedProjectItems.Add(destinationFileName);
      context.BuildContext.SourceMap.Add(destinationFileName, fileName);
    }
  }
}
