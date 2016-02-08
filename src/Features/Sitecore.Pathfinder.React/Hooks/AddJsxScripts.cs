// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using React;
using Sitecore.Configuration;
using Sitecore.Events.Hooks;
using Sitecore.IO;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.React.Hooks
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
                    f = f.Mid(2);
                }

                f = FileUtil.MapPath(f);
                if (!Directory.Exists(f))
                {
                    continue;
                }

                foreach (var fileName in Directory.GetFiles(f, "*.jsx", SearchOption.AllDirectories))
                {
                    var jsx = "~" + FileUtil.UnmapPath(fileName, false);
                    ReactSiteConfiguration.Configuration.SetReuseJavaScriptEngines(true).AddScript(jsx);
                }
            }
        }
    }
}
