namespace Sitecore.Pathfinder.TreeNodes
{
  using Newtonsoft.Json.Linq;
  using Sitecore.Pathfinder.Projects;

  public class JsonPropertyTreeNode : TreeNodeAttribute
  {
    public JsonPropertyTreeNode(ISourceFile sourceFile, JProperty jproperty) : base(jproperty.Name, jproperty.Value.ToString(), new TextSpan(sourceFile, jproperty))
    {
    }
  }
}