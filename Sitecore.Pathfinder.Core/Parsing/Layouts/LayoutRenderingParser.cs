namespace Sitecore.Pathfinder.Parsing.Layouts
{
  using System.ComponentModel.Composition;

  [Export(typeof(IParser))]
  public class LayoutRenderingParser : WebFormsRenderingParser
  {

    public LayoutRenderingParser() : base(".aspx", Constants.Templates.Layout)
    {
    }
  }
}
