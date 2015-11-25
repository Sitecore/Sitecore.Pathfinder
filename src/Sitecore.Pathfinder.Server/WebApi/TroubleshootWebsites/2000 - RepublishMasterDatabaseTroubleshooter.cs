// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Pathfinder.Jobs;
using Sitecore.Publishing;

namespace Sitecore.Pathfinder.WebApi.TroubleshootWebsites
{
    public class RepublishMasterDatabaseTroubleshooter : TroubleshooterBase
    {
        public RepublishMasterDatabaseTroubleshooter() : base(2000)
        {
        }

        public override void Troubleshoot(IAppService app)
        {
            Console.WriteLine(Texts.Republishing_master_database___);
            BackgroundJob.Run("Pathfinder Republish", "Publishing", Republish);
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
