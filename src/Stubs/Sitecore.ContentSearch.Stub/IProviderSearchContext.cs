// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;

namespace Sitecore.ContentSearch
{
    public interface IProviderSearchContext : IDisposable
    {
        [NotNull, ItemNotNull]
        IQueryable<TItem> GetQueryable<TItem>();
    }
}
