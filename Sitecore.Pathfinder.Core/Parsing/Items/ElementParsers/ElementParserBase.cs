namespace Sitecore.Pathfinder.Parsing.Items.ElementParsers
{
  using System.Xml.Linq;

  public abstract class ElementParserBase : IElementParser
  {
    public abstract bool CanParse(ItemParseContext context, XElement element);

    public abstract void Parse(ItemParseContext context, XElement element);
  }
}
