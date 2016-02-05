// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using React;
using Sitecore.Configuration;
using Sitecore.Events.Hooks;
using Sitecore.IO;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.React
{
    public class AddJsxScripts : IHook
    {
        public void Initialize()
        {
            var folders = Settings.GetSetting("Pathfinder.React.JsxFolders");

            foreach (var folder in folders.Split(Constants.Pipe, StringSplitOptions.RemoveEmptyEntries))
            {
                var f = folder;
                if (f.StartsWith("~/"))
                {
                    f = f.Mid(1);
                }

                /*
                if (!FileUtil.FolderExists(f))
                {
                    continue;
                }
                */

                ReactSiteConfiguration.Configuration.SetReuseJavaScriptEngines(true).AddScript(folder + "/*.jsx");
            }
        }
    }
}
