using System.Web.Optimization;
using System.Web.Optimization.React;
using React;
using Sitecore.Events.Hooks;

namespace Sitecore.Pathfinder.React
{
    public class Loader : IHook
    {
        public void Initialize()
        {
            RegisterBundles(BundleTable.Bundles);

            ReactSiteConfiguration.Configuration
              .SetReuseJavaScriptEngines(true)
              .AddScript("~/jsx/HeaderImage.jsx")
              .AddScript("~/jsx/TitleText.jsx");
        }

        private void RegisterBundles(BundleCollection bundleTable)
        {
            var jsxBundle = new BabelBundle("~/bundle/react").IncludeDirectory("~/jsx", "*.jsx");
            bundleTable.Add(jsxBundle);
        }
    }
}