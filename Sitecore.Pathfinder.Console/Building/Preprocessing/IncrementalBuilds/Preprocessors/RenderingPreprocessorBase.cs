namespace Sitecore.Pathfinder.Building.Preprocessing.IncrementalBuilds.Preprocessors
{
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Text.RegularExpressions;
  using Sitecore.Pathfinder.Building.Preprocessing.Data;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;

  public abstract class RenderingPreprocessorBase : PreprocessorBase
  {
    protected RenderingPreprocessorBase([NotNull] string name, [NotNull] string templateName) : base(name)
    {
      this.TemplateName = templateName;
    }

    [NotNull]
    protected string TemplateName { get; set; }

    public override void Execute(IPreprocessingContext context, string fileName)
    {
      var contentDirectory = Path.Combine(context.BuildContext.OutputDirectory, context.ContentDirectory);
      var destinationFileName = Path.Combine(contentDirectory, Path.GetFileName(fileName) ?? string.Empty);
      if (PathHelper.CompareFiles(fileName, destinationFileName))
      {
        return;
      }

      this.Copy(context, fileName, destinationFileName);
      context.BuildContext.ModifiedProjectItems.Add(destinationFileName);
      context.BuildContext.SourceMap.Add(destinationFileName, fileName);

      if (!this.HasItemFile(context, fileName))
      {
        this.CreateItemFile(context, fileName, destinationFileName);
      }
    }

    protected virtual void CreateItemFile([NotNull] IPreprocessingContext context, [NotNull] string fileName, [NotNull] string destinationFileName)
    {
      // todo: removed hardcoded 'content' directory
      var contentRootDirectory = Path.Combine(context.BuildContext.OutputDirectory, "content");
      var serializationDirectory = Path.Combine(context.BuildContext.OutputDirectory, context.SerializationDirectory);

      var serializationFileName = Path.Combine(serializationDirectory, PathHelper.GetFileNameWithoutExtensions(fileName) + ".item.xml");
      var pathField = PathHelper.NormalizeItemPath(PathHelper.UnmapPath(contentRootDirectory, destinationFileName));

      var item = new ProjectItem
      {
        TemplateName = this.TemplateName
      };

      var contents = context.BuildContext.FileSystem.ReadAllText(fileName);
      var placeHolders = this.GetPlaceholders(contents);

      item.Fields.Add(new ProjectField(item, "Path", pathField));
      item.Fields.Add(new ProjectField(item, "Place Holders", string.Join(",", placeHolders)));

      context.BuildContext.FileSystem.CreateDirectory(Path.GetDirectoryName(serializationFileName) ?? string.Empty);

      using (var stream = new StreamWriter(serializationFileName, false, Encoding.UTF8))
      {
        item.WriteItemXml(stream);
      }

      context.BuildContext.ModifiedProjectItems.Add(serializationFileName);
      context.BuildContext.SourceMap.Add(fileName, serializationFileName);
    }

    [NotNull]
    protected abstract IEnumerable<string> GetPlaceholders([NotNull] string contents);

    [NotNull]
    protected virtual IEnumerable<string> GetPlaceholdersFromWebFormsFile([NotNull] string contents)
    {
      var matches = Regex.Matches(contents, "<[^>]*Placeholder[^>]*Key=\"([^\"]*)\"[^>]*>", RegexOptions.IgnoreCase);

      return matches.OfType<Match>().Select(i => i.Groups[1].ToString().Trim());
    }
  }
}
