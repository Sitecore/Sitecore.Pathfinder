// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensions
{
    public static class DatabaseExtensions
    {
        [Diagnostics.NotNull]
        [ItemNotNull]
        public static IEnumerable<Item> GetItemsByTemplate([Diagnostics.NotNull] this Database database, [Diagnostics.NotNull] [ItemNotNull] params ID[] templateId)
        {
            var indexName = "sitecore_" + database.Name.ToLowerInvariant() + "_index";

            var index = ContentSearchManager.GetIndex(indexName);
            using (var context = index.CreateSearchContext())
            {
                var queryable = context.GetQueryable<SearchResultItem>().Where(item => templateId.Contains(item.TemplateId)).ToList();
                var items = queryable.Select(r => database.GetItem(r.ItemId));
                return items.Where(i => i != null && !StandardValuesManager.IsStandardValuesHolder(i) && i.Name != "$name");
            }
        }

    }
}
