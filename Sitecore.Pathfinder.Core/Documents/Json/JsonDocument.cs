namespace Sitecore.Pathfinder.Documents.Json
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

          var r = json.Properties().FirstOrDefault(p => p.Name != "$schema");
          if (r == null)
          {
            return TreeNode.Empty;
          }

          var value = r.Value as JObject;
          if (value == null)
          {
            throw new BuildException(Texts.Text3026, this.ParseContext.Document.SourceFile.SourceFileName, 0, 0);
          }

          this.root = this.Parse(r.Name, value, null);
        }                                   

        return this.root;
      }
    }

    [NotNull]
    protected IParseContext ParseContext { get; }

    [NotNull]
    private ITreeNode Parse([NotNull] string name, [NotNull] JObject jobject, [CanBeNull] ITreeNode parent)
    {
      var treeNode = new JsonTreeNode(this, name, jobject, parent);
      parent?.TreeNodes.Add(treeNode);

      foreach (var property in jobject.Properties())
      {
        switch (property.Value.Type)
        {
          case JTokenType.Object:
            this.Parse(property.Name, property.Value.Value<JObject>(), treeNode);
            break;

          case JTokenType.Array:
            var array = property.Value.Value<JArray>();
            var arrayTreeNode = new JsonTreeNode(this, property.Name, array, parent);

            foreach (var o in array.OfType<JObject>())
            {
              this.Parse(string.Empty, o, arrayTreeNode);
            }

            treeNode.TreeNodes.Add(arrayTreeNode);
            break;

          case JTokenType.Boolean:
          case JTokenType.Date:
          case JTokenType.Float:
          case JTokenType.Integer:
          case JTokenType.String:
            var propertyTreeNode = new JsonTreeNode(this, property.Name, property);
            treeNode.Attributes.Add(propertyTreeNode);
            break;
        }
      }

      return treeNode;
    }
  }
}
