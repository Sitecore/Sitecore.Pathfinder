// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Rainbow.Storage.Sc.Deserialization;
using Rainbow.Storage.Yaml;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Emitting;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Unicorn.Deserialization;

namespace Sitecore.Pathfinder.Unicorn.Languages.Unicorn
{
    public class UnicornFileEmitter : EmitterBase
    {
        [ImportingConstructor]
        public UnicornFileEmitter([NotNull] IFileSystemService fileSystem) : base(Constants.Emitters.Items)
        {
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override bool CanEmit(IEmitContext context, IProjectItem projectItem)
        {
            return projectItem is UnicornFile;
        }

        public override void Emit(IEmitContext context, IProjectItem projectItem)
        {
            var unicornFile = projectItem as UnicornFile;
            Assert.Cast(unicornFile, nameof(unicornFile));

            var snapshot = unicornFile.Snapshot;

            // todo: use real Unicorn configuration instead of hacking it
            var log = new TraceLogger(context.Trace);
            var logger = new DefaultDeserializerLogger(log);
            var fieldFilter = new AllFieldFilter();
            var defaultDeserializer = new DefaultDeserializer(logger, fieldFilter);

            // todo: file has already been read and parsed - do not read it again
            var formatter = new YamlSerializationFormatter(null, null);
            using (var stream = FileSystem.OpenRead(snapshot.SourceFile.AbsoluteFileName))
            {
                var serializedItem = formatter.ReadSerializedItem(stream, unicornFile.ShortName);

                if (string.IsNullOrEmpty(serializedItem.DatabaseName))
                {
                    serializedItem.DatabaseName = unicornFile.DatabaseName;
                }

                try
                {
                    defaultDeserializer.Deserialize(serializedItem);

                    // todo: call UpdateProjectUniqueIds on updated item
                }
                catch (Exception ex)
                {
                    throw new RetryableEmitException(Msg.E1009, Texts.Failed_to_deserialize_item, unicornFile.Snapshot, ex.Message);
                }
            }
        }
    }
}
