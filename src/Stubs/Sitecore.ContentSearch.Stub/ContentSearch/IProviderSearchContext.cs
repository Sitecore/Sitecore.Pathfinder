// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;

namespace Sitecore.ContentSearch
{
    public interface IProviderSearchContext : IDisposable
    {
        [NotNull]
        IQueryable<TItem> GetQueryable<TItem>();
    }
}
