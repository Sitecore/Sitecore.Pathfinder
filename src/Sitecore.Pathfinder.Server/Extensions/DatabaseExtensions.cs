// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Query;
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

        [ItemNotNull]
        [Diagnostics.NotNull]
        public static IEnumerable<Item> Query([Diagnostics.NotNull] this Database database, [Diagnostics.NotNull] string queryText)
        {
            var query = new Query(queryText)
            {
                Max = int.MaxValue
            };

            var result = query.Execute(database.GetRootItem());

            var queryContext = result as QueryContext;
            if (queryContext != null)
            {
                var item = database.GetItem(queryContext.ID);
                if (item != null)
                {
                    yield return item;
                }
            }

            var queryContextArray = result as QueryContext[];
            if (queryContextArray == null)
            {
                yield break;
            }

            foreach (var i in queryContextArray)
            {
                var item = database.GetItem(i.ID);
                if (item != null)
                {
                    yield return item;
                }
            }
    }
    }
}
