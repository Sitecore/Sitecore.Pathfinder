namespace Sitecore.Pathfinder.Parsing.Items.XmlItemParsers
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing.Items.XmlItemParsers.ElementParsers;

  [Export(typeof(IItemParser))]
  public class XmlItemParser : ItemParserBase
  {
    private const string FileExtension = ".item.xml";

    [ImportingConstructor]
    public XmlItemParser([NotNull] ICompositionService compositionService) : base(Items)
    {
      compositionService.SatisfyImportsOnce(this);
    }

    [NotNull]
    [ImportMany]
    public IEnumerable<IElementParser> ElementParsers { get; [UsedImplicitly] private set; }

    public override bool CanParse(IItemParseContext context)
    {
      return context.FileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IItemParseContext context)
    {
      var root = this.LoadXmlFile(context);

      this.ParseElement(context, root);
    }

    public void ParseElement([NotNull] IItemParseContext context, [NotNull] XElement element)
    {
      foreach (var elementParser in this.ElementParsers)
      {
        if (elementParser.CanParse(context, this, element))
        {
          elementParser.Parse(context, this, element);
        }
      }
    }

    public void ParseElements([NotNull] IItemParseContext context, [NotNull] XElement element)
    {
      foreach (var e in element.Elements())
      {
        this.ParseElement(context, e);
      }
    }
  }
}
