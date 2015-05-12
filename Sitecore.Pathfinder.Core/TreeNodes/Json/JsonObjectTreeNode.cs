namespace Sitecore.Pathfinder.TreeNodes.Json
{
  using Newtonsoft.Json.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class JsonObjectTreeNode : TreeNode
  {
    public JsonObjectTreeNode([NotNull] IDocument document, [NotNull] string name, [NotNull] JObject jobject, [CanBeNull] ITreeNode parent = null) : base(name, new TextSpan(document), parent)
    {
    }
  }
}
