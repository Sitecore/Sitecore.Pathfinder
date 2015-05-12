namespace Sitecore.Pathfinder.TreeNodes.Json
{
  using System.Linq;
  using Newtonsoft.Json.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  public class JsonDocument : Document
  {
    private ITreeNode root;

    public JsonDocument([NotNull] IParseContext parseContext, [NotNull] ISourceFile sourceFile) : base(sourceFile)
    {
      this.ParseContext = parseContext;
    }

    public override ITreeNode Root
    {
      get
      {
        if (this.root == null)
        {
          var json = this.SourceFile.ReadAsJson(this.ParseContext);

          var r = json.Properties().FirstOrDefault(j => j.Type == JTokenType.Object);
          if (r == null)
          {
            return TreeNode.Empty;
          }

          this.root = this.Parse(r.Name, r.Value<JObject>(), null);
        }

        return this.root;
      }
    }

    protected IParseContext ParseContext { get; }

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