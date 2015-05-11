namespace Sitecore.Pathfinder.Parsing.Layouts
{
  using System.ComponentModel.Composition;

  [Export(typeof(IParser))]
  public class SublayoutRenderingParser : WebFormsRenderingParser
  {
    public const string SublayoutId = "{0A98E368-CDB9-4E1E-927C-8E0C24A003FB}";

    public SublayoutRenderingParser() : base(".ascx", SublayoutId)
    {
    }
  }
}
