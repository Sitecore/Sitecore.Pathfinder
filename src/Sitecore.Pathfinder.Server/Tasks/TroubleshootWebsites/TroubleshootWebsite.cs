// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Tasks.TroubleshootWebsites
{
    [Export(nameof(TroubleshootWebsite), typeof(IWebsiteTask))]
    public class TroubleshootWebsite : WebsiteTaskBase
    {
        public TroubleshootWebsite() : base("server:troubleshoot-website")
        {
        }

        [Diagnostics.NotNull, ItemNotNull, ImportMany]
        public IEnumerable<ITroubleshooter> Troubleshooters { get; private set; }

        public override void Run(IWebsiteTaskContext context)
        {
            Context.SetActiveSite("shell");

            foreach (var troubleshooter in Troubleshooters.OrderBy(t => t.Priority))
            {
                troubleshooter.Troubleshoot(context.Host);
            }
        }
    }
}
