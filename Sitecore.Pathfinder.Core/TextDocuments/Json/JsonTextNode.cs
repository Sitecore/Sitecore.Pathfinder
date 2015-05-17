namespace Sitecore.Pathfinder.TextDocuments.Json
{
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class JsonTextNode : TextNode
  {
    public JsonTextNode([NotNull] ITextDocument document, [NotNull] string name, [NotNull] JObject jobject, [CanBeNull] ITextNode parent = null) : base(document, name, string.Empty, ((IJsonLineInfo)jobject).LineNumber, ((IJsonLineInfo)jobject).LinePosition, parent)
    {
    }

    public JsonTextNode([NotNull] ITextDocument document, [NotNull] string name, [NotNull] JArray jarray, [CanBeNull] ITextNode parent = null) : base(document, name, string.Empty, ((IJsonLineInfo)jarray).LineNumber, ((IJsonLineInfo)jarray).LinePosition, parent)
    {
    }

    public JsonTextNode([NotNull] ITextDocument document, [NotNull] string name, [NotNull] JProperty jproperty, [CanBeNull] ITextNode parent = null) : base(document, name, jproperty.Value?.ToString() ?? string.Empty, ((IJsonLineInfo)jproperty).LineNumber, ((IJsonLineInfo)jproperty).LinePosition, parent)
    {
    }
  }
}
