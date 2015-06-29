// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Projects.Layouts
{
    public class Rendering : ContentFile
    {
        public Rendering([NotNull] IProject project, [NotNull] ISnapshot snapshot, [NotNull] string filePath, [NotNull] Item item) : base(project, snapshot, filePath)
        {
            Item = item;
        }

        [NotNull]
        public Item Item { get; }
    }
}
