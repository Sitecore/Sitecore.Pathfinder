// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Serialization
{
    [Export(typeof(ISnapshotLoader))]
    public class SerializationSnapshotLoader : ISnapshotLoader
    {
        public SerializationSnapshotLoader()
        {
            Priority = 1000;
        }

        public double Priority { get; }

        public virtual bool CanLoad(ISnapshotService snapshotService, IProject project, ISourceFile sourceFile)
        {
            return string.Equals(Path.GetExtension(sourceFile.AbsoluteFileName), ".item", StringComparison.OrdinalIgnoreCase);
        }

        public virtual ISnapshot Load(ISnapshotService snapshotService, IProject project, ISourceFile sourceFile)
        {
            return new TextSnapshot(sourceFile);
        }
    }
}
