// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Caching;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(nameof(ClearCaches), typeof(IWebsiteTask))]
    public class ClearCaches : WebsiteTaskBase
    {
        [ImportingConstructor]
        public ClearCaches() : base("server:clear-caches")
        {
        }

        public override void Run(IWebsiteTaskContext context)
        {
            var caches = CacheManager.GetAllCaches();

            foreach (var cache in caches)
            {
                cache.Clear();
            }
        }
    }
}
