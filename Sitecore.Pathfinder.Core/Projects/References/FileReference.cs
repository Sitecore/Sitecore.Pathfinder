// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Projects.Files;

namespace Sitecore.Pathfinder.Projects.References
{
    public class FileReference : Reference
    {
        public FileReference([NotNull] IProjectItem owner, [NotNull] string targetQualifiedName) : base(owner, targetQualifiedName)
        {
        }

        public FileReference([NotNull] IProjectItem owner, [NotNull] Attribute<string> sourceAttribute, [NotNull] string targetQualifiedName) : base(owner, sourceAttribute, targetQualifiedName)
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

                var result = Owner.Project.Items.FirstOrDefault(i => i.Guid == TargetProjectItemGuid);
                if (result == null)
                {
                    IsValid = false;
                    TargetProjectItemGuid = Guid.Empty;
                }

                return result;
            }

            IsResolved = true;

            var projectItem = Owner.Project.Items.OfType<File>().FirstOrDefault(i => string.Compare(i.FilePath, TargetQualifiedName, StringComparison.OrdinalIgnoreCase) == 0);
            if (projectItem == null)
            {
                IsValid = false;
                TargetProjectItemGuid = Guid.Empty;
                return null;
            }

            TargetProjectItemGuid = projectItem.Guid;
            IsValid = true;

            return projectItem;
        }
    }
}
