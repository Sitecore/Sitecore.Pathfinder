namespace Sitecore.Pathfinder.Emitters.Files
{
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Configuration;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.Files;
  using Sitecore.Resources.Media;

  [Export(typeof(IEmitter))]
  public class MediaFileEmitter : EmitterBase
  {
    public MediaFileEmitter() : base(MediaFiles)
    {
    }

    public override bool CanEmit(IEmitContext context, IProjectItem projectItem)
    {
      return projectItem is MediaFile;
    }

    public override void Emit(IEmitContext context, IProjectItem projectItem)
    {
      var mediaFile = (MediaFile)projectItem;

      var database = Factory.GetDatabase(mediaFile.MediaItem.DatabaseName);
      var name = mediaFile.MediaItem.ItemName;

      var options = new MediaCreatorOptions
      {
        AlternateText = name, 
        Database = database, 
        FileBased = false, 
        IncludeExtensionInItemName = false, 
        KeepExisting = false, 
        Language = LanguageManager.DefaultLanguage, 
        Versioned = false, 
        Destination = mediaFile.MediaItem.ItemIdOrPath
      };

      // create parent path of media folders before uploading
      var parentPath = PathHelper.NormalizeItemPath(Path.GetDirectoryName(mediaFile.MediaItem.ItemIdOrPath) ?? string.Empty);
      var mediaFolderTemplate = new TemplateItem(database.GetItem(TemplateIDs.MediaFolder));
      database.CreateItemPath(parentPath, mediaFolderTemplate);

      using (var stream = new FileStream(projectItem.TreeNode.Document.SourceFile.SourceFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        var item = MediaManager.Creator.CreateFromStream(stream, "/upload/" + Path.GetFileName(projectItem.TreeNode.Document.SourceFile.SourceFileName), options);
        if (item == null)
        {
          throw new BuildException(Texts.Text2013, projectItem.TreeNode);
        }
      }
    }
  }
}
