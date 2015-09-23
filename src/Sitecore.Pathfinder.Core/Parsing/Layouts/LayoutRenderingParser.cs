// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;

namespace Sitecore.Pathfinder.Parsing.Layouts
{
    [Export(typeof(IParser))]
    public class LayoutRenderingParser : WebFormsRenderingParser
    {
        public LayoutRenderingParser() : base(".aspx", Constants.Templates.Layout)
        {
        }
    }
}
