namespace Sitecore.Pathfinder.Emitters.Files
{
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Configuration;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Emitters.Items;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.Files;
  using Sitecore.Resources.Media;

  [Export(typeof(IEmitter))]
  public class MediaFileEmitter : EmitterBase
  {
    public MediaFileEmitter() : base(Constants.Emitters.MediaFiles)
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
      var name = mediaFile.MediaItem.ItemName.Value;

      var options = new MediaCreatorOptions
      {
        AlternateText = name, 
        Database = database, 
        FileBased = false, 
        IncludeExtensionInItemName = false, 
        // keep existing item - but the KeepExisting flag is flipped - must be an issue in the CMS
        KeepExisting = false, 
        Language = LanguageManager.DefaultLanguage, 
        Versioned = false, 
        Destination = mediaFile.MediaItem.ItemIdOrPath
      };

      var destinationItem = database.GetItem(mediaFile.MediaItem.ItemIdOrPath);

      if (destinationItem != null && destinationItem.ID.ToGuid() != mediaFile.MediaItem.Guid)
      {
        // whoops - item has wrong ID
        destinationItem.Delete();
        destinationItem = null;
      }

      if (destinationItem == null)
      {
        // create parent path of media folders before uploading
        var parentPath = PathHelper.GetItemParentPath(mediaFile.MediaItem.ItemIdOrPath);
        var mediaFolderTemplate = new TemplateItem(database.GetItem(TemplateIDs.MediaFolder));
        var parent = database.CreateItemPath(parentPath, mediaFolderTemplate);

        // create media item with correct ID, but probably wrong template
        ItemManager.AddFromTemplate(name, TemplateIDs.Folder, parent, new ID(mediaFile.MediaItem.Guid));
      }

      using (var stream = new FileStream(projectItem.Snapshot.SourceFile.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        var item = MediaManager.Creator.CreateFromStream(stream, "/upload/" + Path.GetFileName(projectItem.Snapshot.SourceFile.FileName), options);
        if (item == null)
        {
          throw new EmitException(Texts.Failed_to_upload_media, projectItem.Snapshot);
        }

        if (mediaFile.MediaItem.Guid != item.ID.ToGuid())
        {
          context.Trace.TraceError(Texts.Media_item_created_with_wrong_ID, new SnapshotTextNode(mediaFile.Snapshot), $"{item.ID} != {mediaFile.MediaItem.Guid.ToString("B").ToUpperInvariant()}");
        }
      }

      var itemEmitter = new ItemEmitter();
      itemEmitter.Emit(context, mediaFile.MediaItem);
    }
  }
}
