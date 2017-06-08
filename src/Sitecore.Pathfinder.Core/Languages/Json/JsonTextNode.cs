// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Json
{
    public class JsonTextNode : TextNode
    {
        [NotNull, ItemCanBeNull]
        private readonly JToken _jtoken;

        public JsonTextNode([NotNull] ISnapshot snapshot, [NotNull] string key, [NotNull, ItemCanBeNull] JObject jobject, [ItemNotNull, NotNull] IEnumerable<ITextNode> attributes, [ItemNotNull, NotNull] IEnumerable<ITextNode> childNodes) : base(snapshot, key, string.Empty, GetTextSpan(jobject), attributes, childNodes)
        {
            _jtoken = jobject;
        }

        public JsonTextNode([NotNull] ISnapshot snapshot, [NotNull] string key, [NotNull, ItemCanBeNull] JArray jarray, [ItemNotNull, NotNull] IEnumerable<ITextNode> attributes, [ItemNotNull, NotNull] IEnumerable<ITextNode> childNodes) : base(snapshot, key, string.Empty, GetTextSpan(jarray), attributes, childNodes)
        {
            _jtoken = jarray;
        }

        public JsonTextNode([NotNull] ISnapshot snapshot, [NotNull] string key, [NotNull, ItemCanBeNull] JProperty jproperty) : base(snapshot, key, jproperty.Value?.ToString() ?? string.Empty, GetTextSpan(jproperty))
        {
            _jtoken = jproperty;
        }

        public override ITextNode Inner => new JsonInnerTextNode(this, _jtoken);

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
