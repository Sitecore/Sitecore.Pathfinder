// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Extensibility
{
    public interface IExtension
    {
        void RemoveWebsiteFiles([NotNull] IBuildContext context);

        bool UpdateWebsiteFiles([NotNull] IBuildContext context);

        void Start();
    }
}
