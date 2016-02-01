// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.ContentSearch
{
    public interface ISearchIndex
    {
        [NotNull]
        string Name { get; }

        [NotNull]
        IProviderSearchContext CreateSearchContext();

        void Rebuild();
    }
}
