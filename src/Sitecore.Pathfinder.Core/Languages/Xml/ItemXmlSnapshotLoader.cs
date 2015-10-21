// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
    [Export(typeof(ISnapshotLoader))]
    public class ItemXmlSnapshotLoader : XmlSnapshotLoader
    {
        [ImportingConstructor]
        public ItemXmlSnapshotLoader([NotNull] ICompositionService compositionService) : base(compositionService)
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
