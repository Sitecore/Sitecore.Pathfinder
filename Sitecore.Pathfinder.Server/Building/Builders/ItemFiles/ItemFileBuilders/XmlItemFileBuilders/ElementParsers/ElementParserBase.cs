namespace Sitecore.Pathfinder.Building.Builders.ItemFiles.ItemFileBuilders.XmlItemFileBuilders.ElementParsers
{
  using System.Xml.Linq;

  public abstract class ElementParserBase : IElementParser
  {
    public abstract bool CanParse(XmlItemParserContext context, XElement element);

    public abstract void Parse(XmlItemParserContext context, XElement element);
  }
}
