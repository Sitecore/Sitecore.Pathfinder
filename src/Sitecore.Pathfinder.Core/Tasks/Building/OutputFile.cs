// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Tasks.Building
{
    public class OutputFile
    {
        public OutputFile([NotNull] string fileName)
        {
            FileName = fileName;
        }

        [NotNull]
        public string FileName { get; }
    }
}
