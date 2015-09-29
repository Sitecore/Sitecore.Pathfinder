// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
    [Export(typeof(ISnapshotLoader))]
    public class XmlSnapshotLoader : ISnapshotLoader
    {
        public XmlSnapshotLoader()
        {
            Priority = 1000;
        }

        public double Priority { get; protected set; }

        [NotNull]
        public string SchemaFileName { get; protected set; } = string.Empty;

        [NotNull]
        public string SchemaNamespace { get; protected set; } = string.Empty;

        public virtual bool CanLoad(ISnapshotService snapshotService, IProject project, ISourceFile sourceFile)
        {
            return string.Compare(Path.GetExtension(sourceFile.AbsoluteFileName), ".xml", StringComparison.OrdinalIgnoreCase) == 0;
        }

        public virtual ISnapshot Load(ISnapshotService snapshotService, IProject project, ISourceFile sourceFile)
        {
            var text = sourceFile.ReadAsText();

            text = snapshotService.ReplaceTokens(project, sourceFile, text);

            return new XmlTextSnapshot(sourceFile, text, SchemaNamespace, SchemaFileName);
        }
    }
}
