// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Building
{
    [InheritedExport]
    public interface IBuildTask
    {
        [NotNull]
        string TaskName { get; }

        bool CanRunWithoutConfig { get;  }

        void Run([NotNull] IBuildContext context);

        void WriteHelp([NotNull] HelpWriter helpWriter);
    }
}
