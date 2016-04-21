// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Runtime.Caching
{
    public interface ICacheService
    {
        [CanBeNull]
        T Get<T>([NotNull] string cacheKey);

        void Set([NotNull] string cacheKey, [NotNull] object value);
    }
}
