namespace Sitecore.Pathfinder.Parsing.Layouts
{
  using System.ComponentModel.Composition;

  [Export(typeof(IParser))]
  public class LayoutRenderingParser : WebFormsRenderingParser
  {
    public const string LayoutId = "{3A45A723-64EE-4919-9D41-02FD40FD1466}";

    public LayoutRenderingParser() : base(".aspx", LayoutId)
    {
    }
  }
}
