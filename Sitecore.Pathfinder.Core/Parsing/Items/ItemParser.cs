namespace Sitecore.Pathfinder.Parsing.Items
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers;

  [Export(typeof(IParser))]
  public class ItemParser : ParserBase
  {
    private static readonly string[] FileExtensions =
    {
      ".item.xml", 
      ".content.xml", 
      ".item.json", 
      ".content.json", 
      ".layout.json"
    };

    public ItemParser() : base(Constants.Parsers.Items)
    {
    }

    [NotNull]
    [ImportMany]
    public IEnumerable<ITextNodeParser> TextNodeParsers { get; [UsedImplicitly] private set; }

    public override bool CanParse(IParseContext context)
    {
      var fileName = context.Snapshot.SourceFile.FileName;
      return FileExtensions.Any(extension => fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase));
    }

    public override void Parse(IParseContext context)
    {
      var textDocument = (ITextSnapshot)context.Snapshot;

      var textNode = textDocument.Root;
      if (textNode == TextNode.Empty)
      {
        context.Trace.TraceWarning(Texts.Source_file_is_empty, textDocument.SourceFile.FileName, textNode.Position);
        return;
      }

      textDocument.ValidateSchema(context);

      var parentItemPath = PathHelper.GetItemParentPath(context.ItemPath);
      var itemParseContext = context.Factory.ItemParseContext(context, this, parentItemPath);

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
        foreach (var textNodeParser in this.TextNodeParsers.OrderBy(p => p.Priority))
        {
          if (textNodeParser.CanParse(context, textNode))
          {
            textNodeParser.Parse(context, textNode);
            break;
          }
        }
      }
      catch (Exception ex)
      {
        context.ParseContext.Trace.TraceError(string.Empty, context.ParseContext.Snapshot.SourceFile.FileName, TextPosition.Empty, ex.Message);
      }
    }
  }
}
