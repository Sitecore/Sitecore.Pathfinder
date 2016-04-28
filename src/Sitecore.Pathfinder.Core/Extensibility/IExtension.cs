// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Extensibility
{
    [InheritedExport]
    public interface IExtension
    {
        void RemoveWebsiteFiles([NotNull] IBuildContext context);

        bool UpdateWebsiteFiles([NotNull] IBuildContext context);
    }
}
