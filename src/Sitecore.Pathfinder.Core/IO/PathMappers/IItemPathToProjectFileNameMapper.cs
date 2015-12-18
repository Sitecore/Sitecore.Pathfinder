using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.IO.PathMappers
{
    public interface IItemPathToProjectFileNameMapper
    {
        bool TryGetProjectFileName([NotNull] string itemPath, [NotNull] string templateName, [NotNull] out string projectFileName, [NotNull] out string format);

        [NotNull]
        string DatabaseName { get; }

        [NotNull]
        string Format { get; }

        [NotNull]
        string ItemPath { get; }
    }
}