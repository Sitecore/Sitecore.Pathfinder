using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects
{
    public interface IProjectIndexer
    {
        void Add([NotNull] IProjectItem projectItem);

        [CanBeNull]
        T FindQualifiedItem<T>([NotNull] string qualifiedName) where T : class, IProjectItem;
    }
}