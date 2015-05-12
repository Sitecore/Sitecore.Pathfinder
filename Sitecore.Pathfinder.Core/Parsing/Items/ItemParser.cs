namespace Sitecore.Pathfinder.Parsing.Items
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.Pathfinder.Parsing.Items.ElementParsers;
  using Sitecore.Pathfinder.TreeNodes;

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
      return context.Document.SourceFile.SourceFileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context)
    {
      // this.ValidateXmlSchema(context, root, "http://www.sitecore.net/pathfinder/item", "item.xsd");
      var document = context.Document;
      if (document.Root == TreeNode.Empty)
      {
        // todo: report
        return;
      }

      var parentItemPath = context.ItemPath;
      var n = parentItemPath.LastIndexOf('/');
      if (n >= 0)
      {
        parentItemPath = parentItemPath.Left(n);
      }

      var itemParseContext = new ItemParseContext(context, this, parentItemPath);

      this.ParseTreeNode(itemParseContext, document.Root);
    }

    public void ParseTreeNode([NotNull] ItemParseContext context, [NotNull] ITreeNode treeNode)
    {
      try
      {
        foreach (var elementParser in this.ElementParsers)
        {
          if (elementParser.CanParse(context, treeNode))
          {
            elementParser.Parse(context, treeNode);
          }
        }
      }
      catch (BuildException ex)
      {
        context.ParseContext.Project.Trace.TraceError(Texts.Text3013, context.ParseContext.Document.SourceFile.SourceFileName, ex.LineNumber, ex.LinePosition, ex.Message);
      }
      catch (Exception ex)
      {
        context.ParseContext.Project.Trace.TraceError(Texts.Text3013, context.ParseContext.Document.SourceFile.SourceFileName, 0, 0, ex.Message);
      }
    }

    public void ParseTreeNodes([NotNull] ItemParseContext context, [NotNull] ITreeNode treeNode)
    {
      foreach (var childNode in treeNode.TreeNodes)
      {
        this.ParseTreeNode(context, childNode);
      }
    }
  }
}
