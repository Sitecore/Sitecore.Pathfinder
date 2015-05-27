namespace Sitecore.Pathfinder.Emitters.Files
{
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.IO;
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

      var destinationFileName = FileUtil.MapPath(contentFile.FilePath);

      // todo: backup to uninstall folder
      context.FileSystem.CreateDirectory(Path.GetDirectoryName(destinationFileName) ?? string.Empty);
      context.FileSystem.Copy(contentFile.Snapshot.SourceFile.FileName, destinationFileName);
    }
  }
}
