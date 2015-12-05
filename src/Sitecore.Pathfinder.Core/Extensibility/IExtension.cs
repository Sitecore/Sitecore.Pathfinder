// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensibility
{
    [InheritedExport]
    public interface IExtension
    {
        bool UpdateWebsiteFiles([NotNull] IBuildContext context);
    }
}
