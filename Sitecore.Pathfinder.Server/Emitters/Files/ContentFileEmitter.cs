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
    public ContentFileEmitter() : base(Constants.Emitters.BinFiles)
    {
    }

    public override bool CanEmit(IEmitContext context, IProjectItem projectItem)
    {
      return projectItem is ContentFile;
    }

    public override void Emit(IEmitContext context, IProjectItem projectItem)
    {
      var contentFile = (ContentFile)projectItem;

      var destinationFileName = "/" + PathHelper.NormalizeItemPath(PathHelper.UnmapPath(context.Project.ProjectDirectory, contentFile.Document.SourceFile.FileName));
      destinationFileName = FileUtil.MapPath(destinationFileName);

      // todo: backup to uninstall folder
      context.FileSystem.CreateDirectory(Path.GetDirectoryName(destinationFileName) ?? string.Empty);
      context.FileSystem.Copy(contentFile.Document.SourceFile.FileName, destinationFileName);
    }
  }
}
