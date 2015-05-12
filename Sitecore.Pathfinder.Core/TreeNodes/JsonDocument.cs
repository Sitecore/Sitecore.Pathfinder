namespace Sitecore.Pathfinder.TreeNodes
{
  using System.Linq;
  using Newtonsoft.Json.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  public class JsonDocument : Document
  {
    public JsonDocument([NotNull] ISourceFile sourceFile) : base(sourceFile)
    {
    }

    public override ITreeNode Root { get; protected set; } = TreeNode.Empty;

    public void Parse([NotNull] IParseContext context)
    {
      var json = this.SourceFile.ReadAsJson(context);

      var root = json.Properties().FirstOrDefault(j => j.Type == JTokenType.Object);
      if (root == null)
      {
        return;
      }

      this.Root = this.Parse(root.Name, root.Value<JObject>(), null);
    }

    [NotNull]
    private ITreeNode Parse([NotNull] string name, [NotNull] JObject jobject, [CanBeNull] ITreeNode parent)
    {
      var treeNode = new JsonObjectTreeNode(this, name, jobject, parent);
      parent?.TreeNodes.Add(treeNode);

      foreach (var property in jobject.Properties())
      {
        switch (property.Value.Type)
        {
          case JTokenType.Object:
            this.Parse(property.Name, property.Value.Value<JObject>(), treeNode);
            break;

          case JTokenType.Boolean:
          case JTokenType.Date:
          case JTokenType.Float:
          case JTokenType.Integer:
          case JTokenType.String:
            var propertyTreeNode = new JsonPropertyTreeNode(this, property);
            treeNode.Attributes.Add(propertyTreeNode);
            break;
        }
      }

      return treeNode;
    }
  }
}
