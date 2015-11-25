// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Configuration;
using Sitecore.ContentSearch;
using Sitecore.Diagnostics;
using Sitecore.Pathfinder.Jobs;
using Sitecore.Search;

namespace Sitecore.Pathfinder.WebApi.TroubleshootWebsites
{
    public class RebuildClassicIndexesTroubleshooter : TroubleshooterBase
    {
        public RebuildClassicIndexesTroubleshooter() : base(1000)
        {
        }

        public override void Troubleshoot(IAppService app)
        {
            Console.WriteLine(Texts.Rebuilding_indexes___);

            BackgroundJob.Run("Pathfinder Rebuild ContentSearch Indexes", "Indexing", RebuildContentSearchIndexes);
            BackgroundJob.Run("Pathfinder Rebuild Classic Indexes", "Indexing", () => RebuildClassicIndex("master"));
            BackgroundJob.Run("Pathfinder Rebuild Classic Indexes", "Indexing", () => RebuildClassicIndex("core"));
        }

        protected virtual void RebuildClassicIndex([NotNull] string databaseName)
        {
#pragma warning disable 618
            var database = Factory.GetDatabase(databaseName);

            try
            {
                var index = SearchManager.SystemIndex;
                index.Rebuild();
            }
            catch (Exception exception)
            {
                Log.Error("Failed to rebuild system search index", exception, GetType());
            }

            Log.Audit(this, "Rebuild system index: " + databaseName);

            for (var n = 0; n < database.Indexes.Count; n++)
            {
                try
                {
                    database.Indexes[n].Rebuild(database);
                    Log.Audit(this, "Rebuild search index: {0}", database.Name);
                }
                catch (Exception exception)
                {
                    Log.Error("Failed to rebuild search index", exception, GetType());
                }
            }
#pragma warning restore 618
        }

        protected virtual void RebuildContentSearchIndexes()
        {
            foreach (var searchIndex in ContentSearchManager.Indexes)
            {
                Log.Audit(this, "Rebuild Content Search Index: {0}", searchIndex.Name);
                searchIndex.Rebuild();
            }
        }
    }
}
