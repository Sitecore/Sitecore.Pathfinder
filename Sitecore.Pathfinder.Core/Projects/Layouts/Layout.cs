// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Projects.Files;

namespace Sitecore.Pathfinder.Projects.Layouts
{
    public class Layout : ContentFile
    {
        public Layout([NotNull] IProject project, [NotNull] ISnapshot snapshot) : base(project, snapshot)
        {
        }
    }
}
