// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Files;

namespace Sitecore.Pathfinder.Projects.References
{
    public class FileReference : Reference
    {
        public FileReference([NotNull] IProjectItem owner, [NotNull] SourceProperty<string> sourceProperty) : base(owner, sourceProperty)
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

            var filePath = SourceProperty.GetValue();

            var projectItem = Owner.Project.FindFiles<File>(filePath).FirstOrDefault();
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
