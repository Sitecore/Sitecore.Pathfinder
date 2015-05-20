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

    public ItemParser() : base(Constants.Parsers.Items)
    {
    }

    [NotNull]
    [ImportMany]
    public IEnumerable<ITextNodeParser> TextNodeParsers { get; [UsedImplicitly] private set; }

    public override bool CanParse(IParseContext context)
    {
      var fileName = context.Document.SourceFile.FileName;
      return FileExtensions.Any(extension => fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase));
    }

    public override void Parse(IParseContext context)
    {
      var textDocument = context.Document as ITextDocument;
      if (textDocument == null)
      {
        throw new BuildException("public const string Text document expected", context.Document);
      }

      var textNode = textDocument.Root;
      if (textNode == TextNode.Empty)
      {
        context.Trace.TraceWarning("Source file is empty", textDocument.SourceFile.FileName, textNode.Position);
        return;
      }

      // todo: should be dependent on the document, e.g. also validate json documents
      textDocument.ValidateSchema(context, "http://www.sitecore.net/pathfinder/item", "item.xsd");

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
        context.ParseContext.Trace.TraceError("", context.ParseContext.Document.SourceFile.FileName, ex.Position, ex.Details);
      }
      catch (Exception ex)
      {
        context.ParseContext.Trace.TraceError("", context.ParseContext.Document.SourceFile.FileName, TextPosition.Empty, ex.Message);
      }
    }
  }
}