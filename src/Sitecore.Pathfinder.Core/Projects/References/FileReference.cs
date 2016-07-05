// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.References
{
    public class FileReference : Reference
    {
        public FileReference([NotNull] IProjectItem owner, [NotNull] SourceProperty<string> sourceProperty, [NotNull] string referenceText) : base(owner, sourceProperty, referenceText)
        {
        }

        public FileReference([NotNull] IProjectItem owner, [NotNull] ITextNode textNode, [NotNull] string referenceText) : base(owner, textNode, referenceText)
        {
        }

        public override IProjectItem Resolve()
        {
            if (IsResolved)
            {
                if (!IsValid)
                {
                    return null;
                }

                var result = Owner.Project.ProjectItems.FirstOrDefault(i => i.Uri == ResolvedUri);
                if (result == null)
                {
                    IsValid = false;
                    ResolvedUri = null;
                }

                return result;
            }

            IsResolved = true;

            var projectItem = Owner.Project.GetByFileName<File>(ReferenceText).FirstOrDefault();
            if (projectItem == null)
            {
                IsValid = false;
                ResolvedUri = null;
                return null;
            }

            ResolvedUri = projectItem.Uri;
            IsValid = true;

            return projectItem;
        }
    }
}
