namespace Sitecore.Pathfinder.Emitters.Files
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Diagnostics;
  using Sitecore.IO;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.Files;

  [Export(typeof(IEmitter))]
  public class ContentFileEmitter : EmitterBase
  {
    public ContentFileEmitter() : base(BinFiles)
    {
    }

    public override bool CanEmit(IEmitContext context, ProjectItem projectItem)
    {
      return projectItem is ContentFile;
    }

    public override void Emit(IEmitContext context, ProjectItem projectItem)
    {
      var contentFile = (ContentFile)projectItem;

      var destinationFileName = "/" + PathHelper.NormalizeItemPath(PathHelper.UnmapPath(context.Project.ProjectDirectory, contentFile.TextSpan.Document.SourceFile.SourceFileName));
      destinationFileName = FileUtil.MapPath(destinationFileName);

      // todo: backup to uninstall folder
      try
      {
        context.FileSystem.CreateDirectory(Path.GetDirectoryName(destinationFileName) ?? string.Empty);
        context.FileSystem.Copy(contentFile.TextSpan.Document.SourceFile.SourceFileName, destinationFileName);
      }
      catch (Exception ex)
      {
        Log.Error($"Failed to copy assembly: {contentFile.TextSpan.Document.SourceFile.SourceFileName} -> {destinationFileName}", ex);
      }
    }
  }
}
