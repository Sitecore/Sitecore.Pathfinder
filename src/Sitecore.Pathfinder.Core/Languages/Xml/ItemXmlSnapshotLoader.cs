// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
    [Export(typeof(ISnapshotLoader))]
    public class ItemXmlSnapshotLoader : XmlSnapshotLoader
    {
        public ItemXmlSnapshotLoader()
        {
            Priority = 500;
            SchemaNamespace = "http://www.sitecore.net/pathfinder/item";
            SchemaFileName = "item.xsd";
        }

        public override bool CanLoad(ISnapshotService snapshotService, IProject project, ISourceFile sourceFile)
        {
            return sourceFile.AbsoluteFileName.EndsWith(".item.xml", StringComparison.OrdinalIgnoreCase);
        }
    }
}
