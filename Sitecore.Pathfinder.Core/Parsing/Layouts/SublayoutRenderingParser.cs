namespace Sitecore.Pathfinder.Parsing.Layouts
{
  using System.ComponentModel.Composition;

  [Export(typeof(IParser))]
  public class SublayoutRenderingParser : WebFormsRenderingParser
  {
    public SublayoutRenderingParser() : base(".ascx", Constants.Templates.Sublayout)
    {
    }
  }
}
