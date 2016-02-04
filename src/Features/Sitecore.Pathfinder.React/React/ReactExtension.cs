// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.Extensibility;

namespace Sitecore.Pathfinder.React
{
    public class ReactExtension : ExtensionBase
    {
        public override void RemoveWebsiteFiles(IBuildContext context)
        {
            RemoveWebsiteAssembly(context, "Sitecore.Pathfinder.React.dll");
            /*
            RemoveWebsiteAssembly(context, "ClearScript.dll");
            RemoveWebsiteAssembly(context, "JavaScriptEngineSwitcher.Core.dll");
            RemoveWebsiteAssembly(context, "JavaScriptEngineSwitcher.Msie.dll");
            RemoveWebsiteAssembly(context, "JavaScriptEngineSwitcher.V8.dll");
            RemoveWebsiteAssembly(context, "JSPool.dll");
            RemoveWebsiteAssembly(context, "MsieJavaScriptEngine.dll");
            RemoveWebsiteAssembly(context, "React.Core.dll");
            RemoveWebsiteAssembly(context, "React.Web.dll");
            RemoveWebsiteAssembly(context, "React.Web.Mvc4.dll");
            RemoveWebsiteAssembly(context, "VroomJs.dll");
            RemoveWebsiteAssembly(context, "WebActivatorEx.dll");
            */
        }
                                                                             
        public override bool UpdateWebsiteFiles(IBuildContext context)
        {
            var updated = false;

            updated |= CopyToolsFileToWebsiteBinDirectory(context, "Sitecore.Pathfinder.React.dll");
            /*
            updated |= CopyToolsFileToWebsiteBinDirectory(context, "ClearScript.dll");
            updated |= CopyToolsFileToWebsiteBinDirectory(context, "JavaScriptEngineSwitcher.Core.dll");
            updated |= CopyToolsFileToWebsiteBinDirectory(context, "JavaScriptEngineSwitcher.Msie.dll");
            updated |= CopyToolsFileToWebsiteBinDirectory(context, "JavaScriptEngineSwitcher.V8.dll");
            updated |= CopyToolsFileToWebsiteBinDirectory(context, "JSPool.dll");
            updated |= CopyToolsFileToWebsiteBinDirectory(context, "MsieJavaScriptEngine.dll");
            updated |= CopyToolsFileToWebsiteBinDirectory(context, "React.Core.dll");
            updated |= CopyToolsFileToWebsiteBinDirectory(context, "React.Web.dll");
            updated |= CopyToolsFileToWebsiteBinDirectory(context, "React.Web.Mvc4.dll");
            updated |= CopyToolsFileToWebsiteBinDirectory(context, "VroomJs.dll");
            updated |= CopyToolsFileToWebsiteBinDirectory(context, "WebActivatorEx.dll");
            */

            return updated;
        }                               
    }
}