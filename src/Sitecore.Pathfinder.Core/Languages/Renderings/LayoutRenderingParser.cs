// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Languages.Renderings
{
    [Export(typeof(IParser)), Shared]
    public class LayoutRenderingParser : WebFormsRenderingParser
    {
        [ImportingConstructor]
        public LayoutRenderingParser([NotNull] IConfiguration configuration) : base(configuration, ".aspx", Constants.Templates.LayoutId)
        {
        }
    }
}
