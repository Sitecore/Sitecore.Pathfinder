// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Sitecore.Data.Items;
using Sitecore.Data.Serialization;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Files;

namespace Sitecore.Pathfinder.Emitters.Files
{
    [Export(typeof(IEmitter))]
    public class SerializationFileEmitter : EmitterBase
    {
        private static readonly LoadOptions LoadOptions = new LoadOptions
        {
            ForceUpdate = true
        };

        public SerializationFileEmitter() : base(Constants.Emitters.Items)
        {
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
                var item = DoLoadItem(itemModel.Snapshots.First().SourceFile.FileName, LoadOptions);
                if (item == null)
                {
                    throw new RetryableEmitException(Texts.Failed_to_deserialize_item, itemModel.Snapshots.First(), "Item not created");
                }
            }
            catch (Exception ex)
            {
                throw new RetryableEmitException(Texts.Failed_to_deserialize_item, itemModel.Snapshots.First(), ex.Message);
            }
        }

        [Diagnostics.CanBeNull]
        protected virtual Item DoLoadItem([Diagnostics.NotNull] string fileName, [Diagnostics.NotNull] LoadOptions options)
        {
            using (var reader = new StreamReader(System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)))
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
