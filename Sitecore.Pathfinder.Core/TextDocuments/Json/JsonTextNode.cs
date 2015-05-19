namespace Sitecore.Pathfinder.TextDocuments.Json
{
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class JsonTextNode : TextNode
  {
    public JsonTextNode([NotNull] ITextDocument document, [NotNull] string name, [NotNull] JObject jobject, [CanBeNull] ITextNode parent = null) : base(document, name, string.Empty, ((IJsonLineInfo)jobject).LineNumber, ((IJsonLineInfo)jobject).LinePosition + 1, 0, parent)
    {
    }

    public JsonTextNode([NotNull] ITextDocument document, [NotNull] string name, [NotNull] JArray jarray, [CanBeNull] ITextNode parent = null) : base(document, name, string.Empty, ((IJsonLineInfo)jarray).LineNumber, ((IJsonLineInfo)jarray).LinePosition + 1, 0, parent)
    {
    }

    public JsonTextNode([NotNull] ITextDocument document, [NotNull] string name, [NotNull] JProperty jproperty, [CanBeNull] ITextNode parent = null) : base(document, name, jproperty.Value?.ToString() ?? string.Empty, ((IJsonLineInfo)jproperty).LineNumber, ((IJsonLineInfo)jproperty).LinePosition + 1, GetLineLength(jproperty), parent)
    {
    }

    private static int GetLineLength([NotNull] JProperty jproperty)
    {
      var value = jproperty.Value?.ToString() ?? string.Empty;

      // include quotes
      return string.IsNullOrEmpty(value) ? 0 : value.Length + 2;
    }
  }
}
