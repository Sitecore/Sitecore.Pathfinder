// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitters.Items;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Resources.Media;

namespace Sitecore.Pathfinder.Emitters.Files
{
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
            var name = mediaFile.MediaItem.ItemName;

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

            if (destinationItem != null && destinationItem.ID.ToGuid() != mediaFile.MediaItem.Uri.Guid)
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
                ItemManager.AddFromTemplate(name, TemplateIDs.Folder, parent, new ID(mediaFile.MediaItem.Uri.Guid));
            }

            using (var stream = new FileStream(projectItem.Snapshots.First().SourceFile.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var item = MediaManager.Creator.CreateFromStream(stream, "/upload/" + Path.GetFileName(projectItem.Snapshots.First().SourceFile.FileName), options);
                if (item == null)
                {
                    throw new EmitException(Texts.Failed_to_upload_media, projectItem.Snapshots.First());
                }

                if (mediaFile.MediaItem.Uri.Guid != item.ID.ToGuid())
                {
                    context.Trace.TraceError(Texts.Media_item_created_with_wrong_ID, new SnapshotTextNode(mediaFile.Snapshots.First()), $"{item.ID} != {mediaFile.MediaItem.Uri.Guid.Format()}");
                }
            }

            var itemEmitter = new ItemEmitter();
            itemEmitter.Emit(context, mediaFile.MediaItem);
        }
    }
}
