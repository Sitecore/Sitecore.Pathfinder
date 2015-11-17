// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.Linq;
using Rainbow.Storage.Sc.Deserialization;
using Rainbow.Storage.Yaml;
using Sitecore.Pathfinder.Emitting;
using Sitecore.Pathfinder.Projects;
using Unicorn.Deserialization;

namespace Sitecore.Pathfinder.Unicorn.Languages.Unicorn
{
    public class UnicornFileEmitter : EmitterBase
    {
        public UnicornFileEmitter() : base(Constants.Emitters.Items)
        {
        }

        public override bool CanEmit(IEmitContext context, IProjectItem projectItem)
        {
            return projectItem is UnicornFile;
        }

        public override void Emit(IEmitContext context, IProjectItem projectItem)
        {
            var unicornFile = projectItem as UnicornFile;
            if (unicornFile == null)
            {
                return;
            }

            var snapshot = unicornFile.Snapshots.First();

            // todo: use real Unicorn configuration instead of hacking it
            var log = new TraceLogger(context.Trace);
            var logger = new DefaultDeserializerLogger(log);
            var fieldFilter = new AllFieldFilter();
            var defaultDeserializer = new DefaultDeserializer(logger, fieldFilter);

            // todo: file has already been read and parsed - do not read it again
            var formatter = new YamlSerializationFormatter(null, null);
            using (var stream = new FileStream(snapshot.SourceFile.AbsoluteFileName, FileMode.Open))
            {
                var serializedItem = formatter.ReadSerializedItem(stream, unicornFile.ShortName);

                if (string.IsNullOrEmpty(serializedItem.DatabaseName))
                {
                    serializedItem.DatabaseName = unicornFile.DatabaseName;
                }

                defaultDeserializer.Deserialize(serializedItem);
            }
        }
    }
}
