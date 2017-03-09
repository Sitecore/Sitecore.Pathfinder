// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Languages.Renderings
{
    [Export(typeof(IParser)), Shared]
    public class LayoutRenderingParser : WebFormsRenderingParser
    {
        public LayoutRenderingParser() : base(".aspx", Constants.Templates.LayoutId)
        {
        }
    }
}
