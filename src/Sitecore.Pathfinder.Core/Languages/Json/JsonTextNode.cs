// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Json
{
    public class JsonTextNode : TextNode, IMutableTextNode
    {
        [NotNull, ItemCanBeNull]
        private JToken _jtoken;

        public JsonTextNode([NotNull] ISnapshot snapshot, [NotNull] string key, [NotNull, ItemCanBeNull]  JObject jobject) : base(snapshot, key, string.Empty, GetTextSpan(jobject))
        {
            _jtoken = jobject;
        }

        public JsonTextNode([NotNull] ISnapshot snapshot, [NotNull] string key, [NotNull, ItemCanBeNull]  JArray jarray) : base(snapshot, key, string.Empty, GetTextSpan(jarray))
        {
            _jtoken = jarray;
        }

        public JsonTextNode([NotNull] ISnapshot snapshot, [NotNull] string key, [NotNull, ItemCanBeNull]  JProperty jproperty) : base(snapshot, key, jproperty.Value?.ToString() ?? string.Empty, GetTextSpan(jproperty))
        {
            _jtoken = jproperty;
        }

        IList<ITextNode> IMutableTextNode.AttributeList => (IList<ITextNode>)Attributes;

        IList<ITextNode> IMutableTextNode.ChildNodeCollection => (IList<ITextNode>)ChildNodes;

        public override ITextNode GetInnerTextNode()
        {
            return new JsonInnerTextNode(this, _jtoken);
        }

        public override ITextNode GetSnapshotLanguageSpecificChildNode(string name)
        {
            return ChildNodes.FirstOrDefault(n => n.Key == name);
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

        bool IMutableTextNode.SetKey(string newKey)
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

        bool IMutableTextNode.SetValue(string newValue)
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
    }
}
