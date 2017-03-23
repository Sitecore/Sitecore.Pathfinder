// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO.PathMappers
{
    public interface IItemPathToProjectFileNameMapper
    {
        [NotNull]
        string DatabaseName { get; }

        [NotNull]
        string Format { get; }

        [NotNull]
        string ItemPath { get; }

        bool TryGetProjectFileName([NotNull] string itemPath, [NotNull] string templateName, [NotNull] out string projectFileName, [NotNull] out string format);
    }
}
