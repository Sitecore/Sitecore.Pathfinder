namespace Sitecore.Pathfinder.Parsing.Items.ElementParsers
{
  using System.Xml.Linq;
  using Sitecore.Pathfinder.TreeNodes;

  public abstract class ElementParserBase : IElementParser
  {
    public abstract bool CanParse(ItemParseContext context, ITreeNode treeNode);

    public abstract void Parse(ItemParseContext context, ITreeNode treeNode);
  }
}
