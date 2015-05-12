namespace Sitecore.Pathfinder.Parsing.Items.ElementParsers
{
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.TreeNodes;

  public interface IElementParser
  {
    bool CanParse([NotNull] ItemParseContext context, [NotNull] ITreeNode treeNode);

    void Parse([NotNull] ItemParseContext context, [NotNull] ITreeNode treeNode);
  }
}
