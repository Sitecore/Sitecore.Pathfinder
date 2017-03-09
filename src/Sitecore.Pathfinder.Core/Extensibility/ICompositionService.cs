using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensibility
{
    public interface ICompositionService
    {
        [NotNull]
        T Resolve<T>();

        [CanBeNull]
        T Resolve<T>([NotNull] string contractName);

        [NotNull, ItemNotNull]
        IEnumerable<T> ResolveMany<T>();
    }
}
