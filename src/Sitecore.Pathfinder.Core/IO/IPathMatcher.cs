using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO
{
    public interface IPathMatcher
    {
        bool IsExcluded([NotNull] string fileName);

        bool IsIncluded([NotNull] string fileName);

        bool IsMatch([NotNull] string fileName);
    }
}