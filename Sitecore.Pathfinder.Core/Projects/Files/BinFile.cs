// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;

namespace Sitecore.Pathfinder.Projects.Files
{
    public class BinFile : File
    {
        public BinFile([NotNull] IProject project, [NotNull] ISnapshot snapshot) : base(project, snapshot)
        {
        }
    }
}
