// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Pathfinder.Jobs;

namespace Sitecore.Pathfinder.WebApi.TroubleshootWebsites
{
    public class RebuildLinkDatabaseTroubleshooter : TroubleshooterBase
    {
        public RebuildLinkDatabaseTroubleshooter() : base(3000)
        {
        }

        public override void Troubleshoot(IAppService app)
        {
            Console.WriteLine(Texts.Rebuilding_link_database___);
            BackgroundJob.Run("Pathfinder Rebuild Link Database", "Link Database", () => RebuildLinkDatabase("master"));
            BackgroundJob.Run("Pathfinder Rebuild Link Database", "Link Database", () => RebuildLinkDatabase("core"));
        }

        protected virtual void RebuildLinkDatabase([Diagnostics.NotNull] string databaseName)
        {
            var database = Factory.GetDatabase(databaseName);
            Log.Audit(this, "Rebuild link database: {0}", database.Name);

            var linkDatabase = Globals.LinkDatabase;
            linkDatabase.Rebuild(database);
        }
    }
}
