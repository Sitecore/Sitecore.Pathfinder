// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;

namespace Sitecore.Pathfinder.Parsing.Layouts
{
    [Export(typeof(IParser))]
    public class SublayoutRenderingParser : WebFormsRenderingParser
    {
        public SublayoutRenderingParser() : base(".ascx", Constants.Templates.Sublayout)
        {
        }
    }
}
