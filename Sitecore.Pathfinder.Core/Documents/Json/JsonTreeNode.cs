namespace Sitecore.Pathfinder.Documents.Json
{
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class JsonTreeNode : TreeNode
  {
    public JsonTreeNode([NotNull] IDocument document, [NotNull] string name, [NotNull] JObject jobject, [CanBeNull] ITreeNode parent = null) : base(document, name, string.Empty, ((IJsonLineInfo)jobject).LineNumber, ((IJsonLineInfo)jobject).LinePosition, parent)
    {
    }

    public JsonTreeNode([NotNull] IDocument document, [NotNull] string name, [NotNull] JArray jarray, [CanBeNull] ITreeNode parent = null) : base(document, name, string.Empty, ((IJsonLineInfo)jarray).LineNumber, ((IJsonLineInfo)jarray).LinePosition, parent)
    {
    }

    public JsonTreeNode([NotNull] IDocument document, [NotNull] string name, [NotNull] JProperty jproperty, [CanBeNull] ITreeNode parent = null) : base(document, name, jproperty.Value?.ToString() ?? string.Empty, ((IJsonLineInfo)jproperty).LineNumber, ((IJsonLineInfo)jproperty).LinePosition, parent)
    {
    }
  }
}
