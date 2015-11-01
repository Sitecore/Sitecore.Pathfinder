// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using System.Web.Mvc;
using Sitecore.Configuration;
using Sitecore.ContentSearch;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Pathfinder.Jobs;
using Sitecore.Publishing;
using Sitecore.Search;

namespace Sitecore.Pathfinder.WebApi
{
    public class TroubleshootWebsite : IWebApi
    {
        public ActionResult Execute()
        {
            Console.WriteLine("Republishing master database...");
            BackgroundJob.Run("Pathfinder Republish", "Publishing", Republish);

            Console.WriteLine("Rebuilding indexes...");
            BackgroundJob.Run("Pathfinder Rebuild ContentSearch Indexes", "Indexing", RebuildContentSearchIndexes);
            BackgroundJob.Run("Pathfinder Rebuild Classic Indexes", "Indexing", () => RebuildClassicIndex("master"));
            BackgroundJob.Run("Pathfinder Rebuild Classic Indexes", "Indexing", () => RebuildClassicIndex("core"));

            Console.WriteLine("Rebuilding link database...");
            BackgroundJob.Run("Pathfinder Rebuild Link Database", "Link Database", () => RebuildLinkDatabase("master"));
            BackgroundJob.Run("Pathfinder Rebuild Link Database", "Link Database", () => RebuildLinkDatabase("core"));

            return null;
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

        protected virtual void RebuildLinkDatabase([Diagnostics.NotNull] string databaseName)
        {
            var database = Factory.GetDatabase(databaseName);
            Log.Audit(this, "Rebuild link database: {0}", database.Name);

            var linkDatabase = Globals.LinkDatabase;
            linkDatabase.Rebuild(database);
        }

        protected virtual void Republish()
        {
            var database = Factory.GetDatabase("master");

            var publishingTargets = PublishManager.GetPublishingTargets(database);

            var targetDatabases = publishingTargets.Select(target => Factory.GetDatabase(target["Target database"])).ToArray();
            if (!targetDatabases.Any())
            {
                return;
            }

            Log.Audit(this, "Republish: master");
            var languages = LanguageManager.GetLanguages(database).ToArray();
            PublishManager.Republish(database, targetDatabases, languages);
        }
    }
}
