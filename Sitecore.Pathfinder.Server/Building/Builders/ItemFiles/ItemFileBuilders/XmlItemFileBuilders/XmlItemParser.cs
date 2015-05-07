namespace Sitecore.Pathfinder.Building.Builders.ItemFiles.ItemFileBuilders.XmlItemFileBuilders
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Building.Builders.ItemFiles.ItemFileBuilders.XmlItemFileBuilders.ElementParsers;

  public class XmlItemParser
  {
    public XmlItemParser([NotNull] ICompositionService compositionService)
    {
      compositionService.SatisfyImportsOnce(this);
    }

    [NotNull]
    [ImportMany]
    public IEnumerable<IElementParser> ElementParsers { get; [UsedImplicitly] private set; }

    public void ParseElement([NotNull] XmlItemParserContext context, [NotNull] XElement element)
    {
      foreach (var elementParser in this.ElementParsers)
      {
        if (elementParser.CanParse(context, element))
        {
          elementParser.Parse(context, element);
        }
      }
    }

    public void ParseElements([NotNull] XmlItemParserContext context, [NotNull] XElement element)
    {
      foreach (var e in element.Elements())
      {
        this.ParseElement(context, e);
      }
    }
  }
}
