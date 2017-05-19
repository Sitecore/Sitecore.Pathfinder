// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.References
{
    public class FileReference : Reference
    {
        public FileReference([NotNull] IProjectItem owner, [NotNull] SourceProperty<string> sourceProperty, [NotNull] string referenceText) : base(owner, sourceProperty, referenceText, string.Empty)
        {
        }

        public FileReference([NotNull] IProjectItem owner, [NotNull] ITextNode textNode, [NotNull] string referenceText) : base(owner, textNode, referenceText, string.Empty)
        {
        }

        public override IProjectItem Resolve()
        {
            return Owner.Project.Indexes.GetByFileName<File>(ReferenceText).FirstOrDefault();
        }
    }
}
