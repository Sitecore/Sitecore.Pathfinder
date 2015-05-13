namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;

  public interface ITreeNodeParser
  {
    bool CanParse([NotNull] ItemParseContext context, [NotNull] ITreeNode treeNode);

    void Parse([NotNull] ItemParseContext context, [NotNull] ITreeNode treeNode);
  }
}
