namespace Sitecore.Pathfinder.Documents.Json
{
  using Newtonsoft.Json.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class JsonObjectTreeNode : TreeNode
  {
    public JsonObjectTreeNode([NotNull] IDocument document, [NotNull] string name, [NotNull] JObject jobject, [CanBeNull] ITreeNode parent = null) : base(document, name, string.Empty, 0, 0, parent)
    {
    }
    public JsonObjectTreeNode([NotNull] IDocument document, [NotNull] string name, [NotNull] JProperty jproperty, [CanBeNull] ITreeNode parent = null) : base(document, name, string.Empty, 0, 0, parent)
    {
    }
  }
}
