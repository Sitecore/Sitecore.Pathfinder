namespace Sitecore.Pathfinder.Documents.Json
{
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  public class JsonTextNode : TextNode
  {
    public JsonTextNode([NotNull] ITextDocumentSnapshot documentSnapshot, [NotNull] string name, [NotNull] JObject jobject, [CanBeNull] ITextNode parent = null) : base(documentSnapshot, GetPosition(jobject), name, string.Empty, parent)
    {
    }

    public JsonTextNode([NotNull] ITextDocumentSnapshot documentSnapshot, [NotNull] string name, [NotNull] JArray jarray, [CanBeNull] ITextNode parent = null) : base(documentSnapshot, GetPosition(jarray), name, string.Empty, parent)
    {
    }

    public JsonTextNode([NotNull] ITextDocumentSnapshot documentSnapshot, [NotNull] string name, [NotNull] JProperty jproperty, [CanBeNull] ITextNode parent = null) : base(documentSnapshot, GetPosition(jproperty), name, jproperty.Value?.ToString() ?? string.Empty, parent)
    {
    }

    private static TextPosition GetPosition([NotNull] IJsonLineInfo lineInfo)
    {
      var lineLength = 0;

      var jproperty = lineInfo as JProperty;
      if (jproperty != null)
      {
        var value = jproperty.Value?.ToString() ?? string.Empty;
        // include quotes
        lineLength = string.IsNullOrEmpty(value) ? 0 : value.Length + 2;
      }

      return new TextPosition(lineInfo.LineNumber, lineInfo.LinePosition + 1, lineLength);
    }
  }
}
