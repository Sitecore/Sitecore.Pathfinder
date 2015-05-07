namespace Sitecore.Pathfinder.Parsing.Items.XmlItemParsers.ElementParsers
{
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public interface IElementParser
  {
    bool CanParse([NotNull] IItemParseContext context, [NotNull] XmlItemParser parser, [NotNull] XElement element);

    void Parse([NotNull] IItemParseContext context, [NotNull] XmlItemParser parser, [NotNull] XElement element);
  }
}
