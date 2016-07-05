// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Sitecore.Data.Serialization;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitting;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.Serialization;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Emitters.Files
{
    public class SerializationFileEmitter : EmitterBase
    {
        [NotNull]
        protected IFileSystemService FileSystem { get; }

        [Diagnostics.NotNull]
        private static readonly LoadOptions LoadOptions = new LoadOptions
        {
            ForceUpdate = true
        };

        [ImportingConstructor]
        public SerializationFileEmitter([NotNull] IFileSystemService fileSystem) : base(Constants.Emitters.Items)
        {
            FileSystem = fileSystem;
        }

        public override bool CanEmit(IEmitContext context, IProjectItem projectItem)
        {
            return projectItem is SerializationFile;
        }

        public override void Emit(IEmitContext context, IProjectItem projectItem)
        {
            var itemModel = (SerializationFile)projectItem;

            try
            {
                var item = DoLoadItem(itemModel.Snapshot.SourceFile.AbsoluteFileName, LoadOptions);
                if (item == null)
                {
                    throw new RetryableEmitException(Msg.E1011, Texts.Failed_to_deserialize_item, itemModel.Snapshot, "Item not created");
                }

                item.UpdateProjectUniqueIds(context);
            }
            catch (Exception ex)
            {
                throw new RetryableEmitException(Msg.E1012, Texts.Failed_to_deserialize_item, itemModel.Snapshot, ex.Message);
            }
        }

        [Diagnostics.CanBeNull]
        protected virtual Data.Items.Item DoLoadItem([Diagnostics.NotNull] string fileName, [Diagnostics.NotNull] LoadOptions options)
        {
            using (var reader = FileSystem.OpenStreamReader(fileName))
            {
                var disabledLocally = ItemHandler.DisabledLocally;
                try
                {
                    ItemHandler.DisabledLocally = true;
                    return ItemSynchronization.ReadItem(reader, options);
                }
                finally
                {
                    ItemHandler.DisabledLocally = disabledLocally;
                }
            }
        }
    }
}
