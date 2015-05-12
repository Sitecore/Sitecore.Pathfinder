namespace Sitecore.Pathfinder.TreeNodes
{
  using Newtonsoft.Json.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  public class JsonObjectTreeNode : TreeNode
  {
    public JsonObjectTreeNode([NotNull] ISourceFile sourceFile, [NotNull] string name, JObject jobject, [CanBeNull] ITreeNode parent = null) : base(name, new TextSpan(sourceFile, jobject), parent)
    {
    }
  }
}