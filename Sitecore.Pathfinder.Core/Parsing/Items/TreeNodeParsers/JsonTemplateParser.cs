namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Documents.Json;

  [Export(typeof(ITreeNodeParser))]
  public class JsonTemplateParser : TemplateParserBase
  {
    public override bool CanParse(ItemParseContext context, ITreeNode treeNode)
    {
      return treeNode.Name == "Template" && treeNode.Document is JsonDocument;
    }

    protected override ITreeNode GetFieldsTreeNode(ITreeNode treeNode)
    {
      return treeNode.TreeNodes.FirstOrDefault(n => n.Name == "Fields");
    }

    [CanBeNull]
    protected override ITreeNode GetSectionsTreeNode(ITreeNode treeNode)
    {
      return treeNode.TreeNodes.FirstOrDefault(n => n.Name == "Sections");
    }
  }
}
