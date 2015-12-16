// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Mvc;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.WebApi.TroubleshootWebsites
{
    [Export(nameof(TroubleshootWebsite), typeof(IWebApi))]
    public class TroubleshootWebsite : IWebApi
    {
        [Diagnostics.NotNull, ItemNotNull, ImportMany]
        public IEnumerable<ITroubleshooter> Troubleshooters { get; private set; }

        public ActionResult Execute(IAppService app)
        {
            Context.SetActiveSite("shell");

            foreach (var troubleshooter in Troubleshooters.OrderBy(t => t.Priority))
            {
                troubleshooter.Troubleshoot(app);
            }

            return null;
        }
    }
}
