// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Events.Hooks;
using Sitecore.Pathfinder.Install;

namespace Sitecore.Pathfinder.Hooks
{
    public class InitializeSitecorePathfinder : IHook
    {
        public void Initialize()
        {
            PackageWatcher.Initialize();
        }
    }
}
