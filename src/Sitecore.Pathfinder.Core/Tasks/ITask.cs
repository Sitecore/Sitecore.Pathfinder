// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Tasks
{
    public interface ITask
    {
        [NotNull]
        string Alias { get; }

        [NotNull]
        string Shortcut { get; }

        /// <summary>The name of the task.</summary>
        /// <remarks>This should have the format "verb-noun" like PowerShell. See approved PowerShell verbs: https://technet.microsoft.com/en-us/library/ms714428%28v=vs.85%29.aspx</remarks>
        [NotNull]
        string TaskName { get; }

        bool IsHidden { get; }

        void Run([NotNull] ITaskContext context);
    }
}
