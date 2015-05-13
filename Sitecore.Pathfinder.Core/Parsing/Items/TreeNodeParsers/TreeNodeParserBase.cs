namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using Sitecore.Pathfinder.Documents;

  public abstract class TreeNodeParserBase : ITreeNodeParser
  {
    public abstract bool CanParse(ItemParseContext context, ITreeNode treeNode);

    public abstract void Parse(ItemParseContext context, ITreeNode treeNode);
  }
}
