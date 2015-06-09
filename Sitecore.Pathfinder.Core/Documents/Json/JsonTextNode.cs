// © 2015 Sitecore Corporation A/S. All rights reserved.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Documents.Json
{
    public class JsonTextNode : TextNode
    {
        public JsonTextNode([NotNull] ITextSnapshot snapshot, [NotNull] string name, [NotNull] JObject jobject, [CanBeNull] ITextNode parent = null) : base(snapshot, GetPosition(jobject), name, string.Empty, parent)
        {
        }

        public JsonTextNode([NotNull] ITextSnapshot snapshot, [NotNull] string name, [NotNull] JArray jarray, [CanBeNull] ITextNode parent = null) : base(snapshot, GetPosition(jarray), name, string.Empty, parent)
        {
        }

        public JsonTextNode([NotNull] ITextSnapshot snapshot, [NotNull] string name, [NotNull] JProperty jproperty, [CanBeNull] ITextNode parent = null) : base(snapshot, GetPosition(jproperty), name, jproperty.Value?.ToString() ?? string.Empty, parent)
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
