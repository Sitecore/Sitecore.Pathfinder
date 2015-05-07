namespace Sitecore.Pathfinder.Parsing.Items.XmlItemParsers.ElementParsers
{
  using System.Xml.Linq;

  public abstract class ElementParserBase : IElementParser
  {
    public abstract bool CanParse(IItemParseContext context, XmlItemParser parser, XElement element);

    public abstract void Parse(IItemParseContext context, XmlItemParser parser, XElement element);
  }
}
