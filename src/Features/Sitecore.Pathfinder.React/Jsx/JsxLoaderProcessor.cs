// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Web.Optimization;
using System.Web.Optimization.React;
using React;
using Sitecore.Configuration;
using Sitecore.IO;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Pipelines.Loader;

namespace Sitecore.Pathfinder.React.Jsx
{
    public class JsxLoaderProcessor : PipelineProcessorBase<LoaderPipeline>
    {
        public JsxLoaderProcessor() : base(1000)
        {
        }

        protected override void Process(LoaderPipeline pipeline)
        {
            var reactCoreFileName = FileUtil.MapPath("/bin/React.Core.dll");
            if (!FileUtil.FileExists(reactCoreFileName))
            {
                return;
            }

            var bundleName = Settings.GetSetting("Pathfinder.React.BundleName");
            if (string.IsNullOrEmpty(bundleName))
            {
                return;
            }

            var directories = Settings.GetSetting("Pathfinder.React.JsxFolders");
            if (string.IsNullOrEmpty(directories))
            {
                return;
            }

            CreateBundle(bundleName, directories);
        }

        private void CreateBundle(string bundleName, string directories)
        {
            ReactSiteConfiguration.Configuration.SetReuseJavaScriptEngines(true);

            var jsxBundle = new BabelBundle(bundleName);

            foreach (var directory in directories.Split(Constants.Pipe, StringSplitOptions.RemoveEmptyEntries))
            {
                var absoluteDirectory = directory;
                if (absoluteDirectory.StartsWith("~/"))
                {
                    absoluteDirectory = absoluteDirectory.Mid(2);
                }

                absoluteDirectory = FileUtil.MapPath(absoluteDirectory);
                if (!Directory.Exists(absoluteDirectory))
                {
                    continue;
                }

                foreach (var fileName in Directory.GetFiles(absoluteDirectory, "*.jsx", SearchOption.AllDirectories))
                {
                    var jsx = "~" + FileUtil.UnmapPath(fileName, false);
                    ReactSiteConfiguration.Configuration.AddScript(jsx);
                    jsxBundle.Include(jsx);
                }
            }

            BundleTable.Bundles.Add(jsxBundle);
        }
    }
}
