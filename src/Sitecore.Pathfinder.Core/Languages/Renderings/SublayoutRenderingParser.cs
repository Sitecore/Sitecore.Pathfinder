// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Languages.Renderings
{
    [Export(typeof(IParser)), Shared]
    public class SublayoutRenderingParser : WebFormsRenderingParser
    {
        public SublayoutRenderingParser() : base(".ascx", Constants.Templates.SublayoutId)
        {
        }
    }
}
