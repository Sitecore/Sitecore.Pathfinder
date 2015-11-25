// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.WebApi.TroubleshootWebsites
{
    [InheritedExport]
    public interface ITroubleshooter
    {
        double Priority { get; }

        void Troubleshoot([NotNull] IAppService app);
    }
}
