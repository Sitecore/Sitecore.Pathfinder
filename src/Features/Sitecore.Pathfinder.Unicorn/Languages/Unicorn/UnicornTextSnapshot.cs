// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using Rainbow.Storage.Sc.Deserialization;
using Rainbow.Storage.Yaml;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;
using Unicorn.Data.DataProvider;

namespace Sitecore.Pathfinder.Unicorn.Languages.Unicorn
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UnicornTextSnapshot : TextSnapshot
    {
        [ImportingConstructor]
        public UnicornTextSnapshot([NotNull] ISnapshotService snapshotService) : base(snapshotService)
        {
        }

        public override ITextNode Root => null;

        [NotNull]
        protected SnapshotParseContext ParseContext { get; private set; }

        [NotNull]
        protected IProject Project { get; private set; }

        public override void SaveChanges()
        {
        }

        [NotNull]
        public virtual UnicornTextSnapshot With([NotNull] SnapshotParseContext parseContext, [NotNull] ISourceFile sourceFile, [NotNull] string contents)
        {
            base.With(sourceFile);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
            {
                using (var reader = new YamlReader(stream, 4096, false))
                {
                }
            }

            return this;
        }
    }
}
