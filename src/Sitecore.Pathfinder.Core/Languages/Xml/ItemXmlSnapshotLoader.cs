// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
    [Export(typeof(ISnapshotLoader)), Shared]
    public class ItemXmlSnapshotLoader : XmlSnapshotLoader
    {
        [ImportingConstructor]
        public ItemXmlSnapshotLoader([NotNull] ICompositionService compositionService, [NotNull] IFileSystemService fileSystem) : base(compositionService, fileSystem)
        {
            Priority = 500;
            SchemaNamespace = "http://www.sitecore.net/pathfinder/item";
            SchemaFileName = "item.xsd";
        }

        public override bool CanLoad(ISourceFile sourceFile)
        {
            return sourceFile.AbsoluteFileName.EndsWith(".item.xml", StringComparison.OrdinalIgnoreCase);
        }
    }
}
