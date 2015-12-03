// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitters.Items;
using Sitecore.Pathfinder.Emitting;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.Media;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Resources.Media;

namespace Sitecore.Pathfinder.Emitters.Files
{
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

            var mediaItem = context.Project.FindQualifiedItem<Projects.Items.Item>(mediaFile.MediaItemUri);
            if (mediaItem == null)
            {
                context.Trace.TraceError(Msg.E1004, Texts.Media_item_not_found, new SnapshotTextNode(mediaFile.Snapshots.First()), mediaFile.MediaItemUri.Guid.Format());
                return;
            }

            if (mediaFile.UploadMedia)
            {
                UploadFile(context, mediaItem, mediaFile);
                return;
            }

            CopyFile(context, mediaFile);
        }

        protected virtual void CopyFile([Diagnostics.NotNull] IEmitContext context, [Diagnostics.NotNull] MediaFile mediaFile)
        {
            var destinationFileName = FileUtil.MapPath(mediaFile.FilePath);

            context.FileSystem.CreateDirectory(Path.GetDirectoryName(destinationFileName) ?? string.Empty);
            context.FileSystem.Copy(mediaFile.Snapshots.First().SourceFile.AbsoluteFileName, destinationFileName, context.ForceUpdate);
        }

        protected virtual void UploadFile([Diagnostics.NotNull] IEmitContext context, [Diagnostics.NotNull] Projects.Items.Item mediaItem, [Diagnostics.NotNull] MediaFile mediaFile)
        {
            var database = Factory.GetDatabase(mediaItem.DatabaseName);
            var name = mediaItem.ItemName;

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
                Destination = mediaItem.ItemIdOrPath
            };

            var destinationItem = database.GetItem(mediaItem.ItemIdOrPath);

            if (destinationItem != null && destinationItem.ID.ToGuid() != mediaItem.Uri.Guid)
            {
                // whoops - item has wrong ID
                destinationItem.Delete();
                destinationItem = null;
            }

            var uploadMedia = true;
            if (!context.ForceUpdate && destinationItem != null)
            {
                var item = new MediaItem(destinationItem);
                var fileInfo = new FileInfo(mediaFile.Snapshots.First().SourceFile.AbsoluteFileName);

                uploadMedia = (item.Size != fileInfo.Length);
            }

            if (destinationItem == null)
            {
                // create parent path of media folders before uploading
                var parentPath = PathHelper.GetItemParentPath(mediaItem.ItemIdOrPath);
                var mediaFolderTemplate = new TemplateItem(database.GetItem(TemplateIDs.MediaFolder));
                var parent = database.CreateItemPath(parentPath, mediaFolderTemplate);

                // create media item with correct ID, but probably wrong template
                ItemManager.AddFromTemplate(name, TemplateIDs.Folder, parent, new Data.ID(mediaItem.Uri.Guid));
            }

            if (uploadMedia)
            {
                using (var stream = new FileStream(mediaFile.Snapshots.First().SourceFile.AbsoluteFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var item = MediaManager.Creator.CreateFromStream(stream, "/upload/" + Path.GetFileName(mediaFile.Snapshots.First().SourceFile.AbsoluteFileName), options);
                    if (item == null)
                    {
                        throw new EmitException(Texts.Failed_to_upload_media, mediaFile.Snapshots.First());
                    }

                    if (mediaItem.Uri.Guid != item.ID.ToGuid())
                    {
                        context.Trace.TraceError(Msg.E1005, Texts.Media_item_created_with_wrong_ID, new SnapshotTextNode(mediaFile.Snapshots.First()), $"{item.ID} != {mediaItem.Uri.Guid.Format()}");
                    }
                }
            }

            var itemEmitter = new ItemEmitter();
            itemEmitter.Emit(context, mediaItem);
        }
    }
}
