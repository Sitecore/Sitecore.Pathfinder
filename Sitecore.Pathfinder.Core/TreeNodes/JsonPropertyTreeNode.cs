namespace Sitecore.Pathfinder.TreeNodes
{
  using Newtonsoft.Json.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class JsonPropertyTreeNode : TreeNodeAttribute
  {
    public JsonPropertyTreeNode([NotNull] IDocument document, [NotNull] JProperty jproperty) : base(jproperty.Name, jproperty.Value.ToString(), new TextSpan(document))
    {
    }
  }
}
