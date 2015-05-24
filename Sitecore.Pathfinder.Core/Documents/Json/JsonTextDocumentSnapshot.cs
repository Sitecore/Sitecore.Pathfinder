namespace Sitecore.Pathfinder.Documents.Json
{
  using System.Linq;
  using Newtonsoft.Json.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class JsonTextDocumentSnapshot : TextDocumentSnapshot
  {
    private ITextNode root;

    public JsonTextDocumentSnapshot([NotNull] ISourceFile sourceFile, [NotNull] string contents) : base(sourceFile, contents)
    {
    }

    public override ITextNode Root
    {
      get
      {
        if (this.root == null)
        {
          JObject json;
          try
          {
            json = JObject.Parse(this.Contents);
          }
          catch
          {
            return TextNode.Empty;
          }

          var r = json.Properties().FirstOrDefault(p => p.Name != "$schema");
          if (r == null)
          {
            return TextNode.Empty;
          }

          var value = r.Value as JObject;
          if (value == null)
          {
            return TextNode.Empty;
          }

          this.root = this.Parse(r.Name, value, null);
        }

        return this.root;
      }
    }

    [NotNull]
    protected virtual ITextNode Parse([NotNull] string name, [NotNull] JObject jobject, [CanBeNull] ITextNode parent)
    {
      var treeNode = new JsonTextNode(this, name, jobject, parent);
      parent?.ChildNodes.Add(treeNode);

      foreach (var property in jobject.Properties())
      {
        switch (property.Value.Type)
        {
          case JTokenType.Object:
            this.Parse(property.Name, property.Value.Value<JObject>(), treeNode);
            break;

          case JTokenType.Array:
            var array = property.Value.Value<JArray>();
            var arrayTreeNode = new JsonTextNode(this, property.Name, array, parent);

            foreach (var o in array.OfType<JObject>())
            {
              this.Parse(string.Empty, o, arrayTreeNode);
            }

            treeNode.ChildNodes.Add(arrayTreeNode);
            break;

          case JTokenType.Boolean:
          case JTokenType.Date:
          case JTokenType.Float:
          case JTokenType.Integer:
          case JTokenType.String:
            var propertyTreeNode = new JsonTextNode(this, property.Name, property);
            treeNode.Attributes.Add(propertyTreeNode);
            break;
        }
      }

      return treeNode;
    }
  }
}
