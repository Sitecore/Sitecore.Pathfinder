// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Tasks.Building
{
    public class OutputFile
    {
        [FactoryConstructor]
        public OutputFile([NotNull] string fileName)
        {
            FileName = fileName;
        }

        [NotNull]
        public string FileName { get; }
    }
}
