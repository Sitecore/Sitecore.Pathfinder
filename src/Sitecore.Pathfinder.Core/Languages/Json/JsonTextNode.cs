// © 2015 Sitecore Corporation A/S. All rights reserved.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Json
{
    public class JsonTextNode : TextNode
    {
        [NotNull]
        [ItemCanBeNull]
        private JToken _jtoken;

        public JsonTextNode([NotNull] ITextSnapshot snapshot, [NotNull] string key, [NotNull] [ItemCanBeNull] JObject jobject, [CanBeNull] ITextNode parentNode = null) : base(snapshot, key, string.Empty, GetTextSpan(jobject), parentNode)
        {
            _jtoken = jobject;
        }

        public JsonTextNode([NotNull] ITextSnapshot snapshot, [NotNull] string key, [NotNull] [ItemCanBeNull] JArray jarray, [CanBeNull] ITextNode parentNode = null) : base(snapshot, key, string.Empty, GetTextSpan(jarray), parentNode)
        {
            _jtoken = jarray;
        }

        public JsonTextNode([NotNull] ITextSnapshot snapshot, [NotNull] string key, [NotNull] [ItemCanBeNull] JProperty jproperty, [CanBeNull] ITextNode parentNode = null) : base(snapshot, key, jproperty.Value?.ToString() ?? string.Empty, GetTextSpan(jproperty), parentNode)
        {
            _jtoken = jproperty;
        }

        public override ITextNode GetInnerTextNode()
        {
            return new JsonInnerTextNode(this, _jtoken);
        }

        public override bool SetKey(string newKey)
        {
            var property = _jtoken as JProperty;
            if (property != null)
            {
                var newProperty = new JProperty(newKey, property.Value);
                property.Replace(newProperty);
                _jtoken = newProperty;

                Snapshot.IsModified = true;
                return true;
            }

            var jobject = _jtoken as JObject;
            if (jobject != null)
            {
                var prop = jobject.Parent as JProperty;
                if (prop != null)
                {
                    var newProperty = new JProperty(newKey, jobject);
                    prop.Replace(newProperty);
                    _jtoken = newProperty;

                    Snapshot.IsModified = true;
                    return true;
                }
            }

            return false;
        }

        public override bool SetValue(string newValue)
        {
            var property = _jtoken as JProperty;
            if (property != null)
            {
                property.Value = newValue;
                Snapshot.IsModified = true;
                return true;
            }

            return false;
        }

        private static TextSpan GetTextSpan([NotNull] IJsonLineInfo lineInfo)
        {
            var lineLength = 0;

            var jproperty = lineInfo as JProperty;
            if (jproperty != null)
            {
                var value = jproperty.Value?.ToString() ?? string.Empty;

                // include quotes
                lineLength = string.IsNullOrEmpty(value) ? 0 : value.Length + 2;
            }

            return new TextSpan(lineInfo.LineNumber, lineInfo.LinePosition + 1, lineLength);
        }
    }
}
