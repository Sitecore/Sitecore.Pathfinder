namespace Sitecore.Pathfinder.Parsing.Items
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers;
  using Sitecore.Pathfinder.TextDocuments;

  [Export(typeof(IParser))]
  public class ItemParser : ParserBase
  {
    private static readonly string[] FileExtensions =
    {
      ".item.xml", 
      ".item.json"
    };

    public ItemParser() : base(Items)
    {
    }

    [NotNull]
    [ImportMany]
    public IEnumerable<ITextNodeParser> TextNodeParsers { get; [UsedImplicitly] private set; }

    public override bool CanParse(IParseContext context)
    {
      var fileName = context.TextDocument.SourceFile.SourceFileName;
      return FileExtensions.Any(extension => fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase));
    }

    public override void Parse(IParseContext context)
    {
      var textNode = context.TextDocument.Root;
      if (textNode == TextNode.Empty)
      {
        context.Project.Trace.TraceWarning(Texts.Text3025, textNode.TextDocument.SourceFile.SourceFileName, textNode.LineNumber, textNode.LinePosition);
        return;
      }

      // todo: should be dependent on the document, e.g. also validate json documents
      context.TextDocument.ValidateSchema(context, "http://www.sitecore.net/pathfinder/item", "item.xsd");

      var parentItemPath = PathHelper.GetItemParentPath(context.ItemPath);
      var itemParseContext = new ItemParseContext(context, this, parentItemPath);

      this.ParseTextNode(itemParseContext, textNode);
    }

    public virtual void ParseChildNodes([NotNull] ItemParseContext context, [NotNull] ITextNode textNode)
    {
      foreach (var childNode in textNode.ChildNodes)
      {
        this.ParseTextNode(context, childNode);
      }
    }

    public virtual void ParseTextNode([NotNull] ItemParseContext context, [NotNull] ITextNode textNode)
    {
      try
      {
        foreach (var textNodeParser in this.TextNodeParsers)
        {
          if (textNodeParser.CanParse(context, textNode))
          {
            textNodeParser.Parse(context, textNode);
          }
        }
      }
      catch (BuildException ex)
      {
        context.ParseContext.Project.Trace.TraceError(Texts.Text3013, context.ParseContext.TextDocument.SourceFile.SourceFileName, ex.LineNumber, ex.LinePosition, ex.Message);
      }
      catch (Exception ex)
      {
        context.ParseContext.Project.Trace.TraceError(Texts.Text3013, context.ParseContext.TextDocument.SourceFile.SourceFileName, 0, 0, ex.Message);
      }
    }
  }
}