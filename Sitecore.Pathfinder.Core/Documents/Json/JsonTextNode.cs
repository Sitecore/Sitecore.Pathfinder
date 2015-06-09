// © 2015 Sitecore Corporation A/S. All rights reserved.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Documents.Json
{
    public class JsonTextNode : TextNode
    {
        private readonly JToken _jtoken;

        public JsonTextNode([NotNull] ITextSnapshot snapshot, [NotNull] string name, [NotNull] JObject jobject, [CanBeNull] ITextNode parent = null) : base(snapshot, GetPosition(jobject), name, string.Empty, parent)
        {
            _jtoken = jobject;
        }

        public JsonTextNode([NotNull] ITextSnapshot snapshot, [NotNull] string name, [NotNull] JArray jarray, [CanBeNull] ITextNode parent = null) : base(snapshot, GetPosition(jarray), name, string.Empty, parent)
        {
            _jtoken = jarray;
        }

        public JsonTextNode([NotNull] ITextSnapshot snapshot, [NotNull] string name, [NotNull] JProperty jproperty, [CanBeNull] ITextNode parent = null) : base(snapshot, GetPosition(jproperty), name, jproperty.Value?.ToString() ?? string.Empty, parent)
        {
            _jtoken = jproperty;
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

        public override bool SetValue(string value)
        {
            var property = _jtoken as JProperty;
            if (property != null)
            {
                property.Value = value;
                Snapshot.IsModified = true;
                return true;
            }

            return false;
        }
    }
}
