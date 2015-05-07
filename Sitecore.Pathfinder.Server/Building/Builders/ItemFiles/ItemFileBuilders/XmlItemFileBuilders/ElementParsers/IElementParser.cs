namespace Sitecore.Pathfinder.Building.Builders.ItemFiles.ItemFileBuilders.XmlItemFileBuilders.ElementParsers
{
  using System.Xml.Linq;

  public interface IElementParser
  {
    bool CanParse([NotNull] XmlItemParserContext context, [NotNull] XElement element);

    void Parse([NotNull] XmlItemParserContext context, [NotNull] XElement element);
  }
}
