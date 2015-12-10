// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Building
{
    [InheritedExport]
    public interface IBuildTask
    {
        /// <summary>The name of the task.</summary>
        /// <remarks>This should have the format "verb-noun" like PowerShell. See approved PowerShell verbs: https://technet.microsoft.com/en-us/library/ms714428%28v=vs.85%29.aspx</remarks>
        [NotNull]
        string TaskName { get; }

        /// <summary>
        /// Get a value that indicates if the task can run, if there is no project scconfig.json.
        /// </summary>
        bool CanRunWithoutConfig { get;  }

        void Run([NotNull] IBuildContext context);

        void WriteHelp([NotNull] HelpWriter helpWriter);
    }
}
