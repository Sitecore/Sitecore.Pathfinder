// © 2015 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.WebApi.TroubleshootWebsites
{
    public abstract class TroubleshooterBase : ITroubleshooter
    {
        protected TroubleshooterBase(double priority)
        {
            Priority = priority;
        }

        public double Priority { get; }

        public abstract void Troubleshoot(IAppService app);
    }
}
