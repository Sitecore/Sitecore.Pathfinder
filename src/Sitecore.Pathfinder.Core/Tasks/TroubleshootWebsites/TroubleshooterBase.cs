// © 2015 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.Tasks.TroubleshootWebsites
{
    public abstract class TroubleshooterBase : ITroubleshooter
    {
        protected TroubleshooterBase(double priority)
        {
            Priority = priority;
        }

        public double Priority { get; }

        public abstract void Troubleshoot(IHostService host);
    }
}
