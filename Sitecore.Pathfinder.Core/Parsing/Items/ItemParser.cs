namespace Sitecore.Pathfinder.Parsing.Items
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Xml.Linq;
  using System.Xml.Schema;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.Pathfinder.Parsing.Items.ElementParsers;

  [Export(typeof(IParser))]
  public class ItemParser : ParserBase
  {
    private const string FileExtension = ".item.xml";

    public ItemParser() : base(Items)
    {
    }

    [NotNull]
    [ImportMany]
    public IEnumerable<IElementParser> ElementParsers { get; [UsedImplicitly] private set; }

    public override bool CanParse(IParseContext context)
    {
      return context.SourceFile.SourceFileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context)
    {
      var root = context.SourceFile.ReadAsXml(context);

      this.ValidateXmlSchema(context, root, "http://www.sitecore.net/pathfinder/item", "item.xsd");

      var parentItemPath = context.ItemPath;

      var n = parentItemPath.LastIndexOf('/');
      if (n >= 0)
      {
        parentItemPath = parentItemPath.Left(n);
      }

      var itemParseContext = new ItemParseContext(context, this, parentItemPath);

      this.ParseElement(itemParseContext, root);
    }

    public void ParseElement([NotNull] ItemParseContext context, [NotNull] XElement element)
    {
      try
      {
        foreach (var elementParser in this.ElementParsers)
        {
          if (elementParser.CanParse(context, element))
          {
            elementParser.Parse(context, element);
          }
        }
      }
      catch (BuildException ex)
      {
        context.ParseContext.Project.Trace.TraceError(Texts.Text3013, context.ParseContext.SourceFile.SourceFileName, ex.Line, ex.Column, ex.Message);
      }
      catch (Exception ex)
      {
        context.ParseContext.Project.Trace.TraceError(Texts.Text3013, context.ParseContext.SourceFile.SourceFileName, 0, 0, ex.Message);
      }
    }

    public void ParseElements([NotNull] ItemParseContext context, [NotNull] XElement element)
    {
      foreach (var e in element.Elements())
      {
        this.ParseElement(context, e);
      }
    }
  }
}
