namespace Sitecore.Pathfinder.TreeNodes
{
  using System.Linq;
  using Newtonsoft.Json.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  public class JsonDocument : IDocument
  {
    public JsonDocument([NotNull] ISourceFile sourceFile)
    {
      this.SourceFile = sourceFile;
    }

    public ITreeNode Root { get; private set; } = TreeNode.Empty;

    public ISourceFile SourceFile { get; }

    public void Parse(IParseContext context)
    {
      var json = this.SourceFile.ReadAsJson(context);

      var root = json.Properties().FirstOrDefault(j => j.Type == JTokenType.Object);
      if (root == null)
      {
        return;
      }

      this.Root = this.Parse(root.Name, root.Value<JObject>(), null);
    }

    private ITreeNode Parse([NotNull] string name, [NotNull] JObject jobject, [CanBeNull] ITreeNode parent)
    {
      var treeNode = new JsonObjectTreeNode(this.SourceFile, name, jobject, parent);
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
            var propertyTreeNode = new JsonPropertyTreeNode(this.SourceFile, property);
            treeNode.Attributes.Add(propertyTreeNode);
            break;
        }
      }

      return treeNode;
    }
  }
}