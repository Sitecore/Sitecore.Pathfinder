// © 2015 Sitecore Corporation A/S. All rights reserved.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots.Json
{
    public class JsonTextNode : TextNode
    {
        [NotNull]
        [ItemCanBeNull]
        private JToken _jtoken;

        public JsonTextNode([NotNull] ITextSnapshot snapshot, [NotNull] string name, [NotNull][ItemCanBeNull] JObject jobject, [CanBeNull] ITextNode parent = null) : base(snapshot, GetPosition(jobject), name, string.Empty, parent)
        {
            _jtoken = jobject;
        }

        public JsonTextNode([NotNull] ITextSnapshot snapshot, [NotNull] string name, [NotNull][ItemCanBeNull] JArray jarray, [CanBeNull] ITextNode parent = null) : base(snapshot, GetPosition(jarray), name, string.Empty, parent)
        {
            _jtoken = jarray;
        }

        public JsonTextNode([NotNull] ITextSnapshot snapshot, [NotNull] string name, [NotNull][ItemCanBeNull] JProperty jproperty, [CanBeNull] ITextNode parent = null) : base(snapshot, GetPosition(jproperty), name, jproperty.Value?.ToString() ?? string.Empty, parent)
        {
            _jtoken = jproperty;
        }

        private static TextSpan GetPosition([NotNull] IJsonLineInfo lineInfo)
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

        public override ITextNode GetInnerTextNode()
        {
            return new JsonInnerTextNode(this, _jtoken);
        }

        public override bool SetName(string newName)
        {
            var property = _jtoken as JProperty;
            if (property != null)
            {
                var newProperty = new JProperty(newName, property.Value);
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
                    var newProperty = new JProperty(newName, jobject);
                    prop.Replace(newProperty);
                    _jtoken = newProperty;

                    Snapshot.IsModified = true;
                    return true;
                }
            }

            return false;
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
