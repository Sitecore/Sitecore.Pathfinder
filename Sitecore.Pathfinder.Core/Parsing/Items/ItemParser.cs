namespace Sitecore.Pathfinder.Parsing.Items
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers;

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
    public IEnumerable<ITreeNodeParser> TreeNodeParsers { get; [UsedImplicitly] private set; }

    public override bool CanParse(IParseContext context)
    {
      var fileName = context.Document.SourceFile.SourceFileName;
      return FileExtensions.Any(extension => fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase));
    }

    public override void Parse(IParseContext context)
    {
      var treeNode = context.Document.Root;
      if (treeNode == TreeNode.Empty)
      {
        context.Project.Trace.TraceWarning(Texts.Text3025, treeNode.Document.SourceFile.SourceFileName, treeNode.LineNumber, treeNode.LinePosition);
        return;
      }

      context.Document.ValidateSchema(context, "http://www.sitecore.net/pathfinder/item", "item.xsd");

      var parentItemPath = this.GetParentItemPath(context);
      var itemParseContext = new ItemParseContext(context, this, parentItemPath);

      this.ParseTreeNode(itemParseContext, treeNode);
    }

    public virtual void ParseTreeNode([NotNull] ItemParseContext context, [NotNull] ITreeNode treeNode)
    {
      try
      {
        foreach (var treeNodeParser in this.TreeNodeParsers)
        {
          if (treeNodeParser.CanParse(context, treeNode))
          {
            treeNodeParser.Parse(context, treeNode);
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

    public virtual void ParseTreeNodes([NotNull] ItemParseContext context, [NotNull] ITreeNode treeNode)
    {
      foreach (var childNode in treeNode.TreeNodes)
      {
        this.ParseTreeNode(context, childNode);
      }
    }

    [NotNull]
    protected virtual string GetParentItemPath([NotNull] IParseContext context)
    {
      var parentItemPath = context.ItemPath;
      var n = parentItemPath.LastIndexOf('/');
      if (n >= 0)
      {
        parentItemPath = parentItemPath.Left(n);
      }

      return parentItemPath;
    }
  }
}
