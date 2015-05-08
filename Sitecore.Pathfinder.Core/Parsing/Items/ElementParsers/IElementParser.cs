namespace Sitecore.Pathfinder.Parsing.Items.ElementParsers
{
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public interface IElementParser
  {
    bool CanParse([NotNull] ItemParseContext context, [NotNull] XElement element);

    void Parse([NotNull] ItemParseContext context, [NotNull] XElement element);
  }
}
