// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensibility
{
    public interface IExtension
    {
        void RemoveWebsiteFiles([NotNull] IExtensionContext context);
                                                                                         
        bool UpdateWebsiteFiles([NotNull] IExtensionContext context);

        void Start();
    }
}
