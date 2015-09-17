// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots.Yaml
{
    public partial class YamlTextSnapshot
    {
        protected class Token
        {
            public Token([NotNull] string key, TextSpan keyTextSpan, [NotNull] string value, TextSpan valueTextSpan, int indent, bool isNested)
            {
                Key = key;
                KeyTextSpan = keyTextSpan;
                Value = value;
                ValueTextSpan = valueTextSpan;
                Indent = indent;
                IsNested = isNested;
            }

            public Token([NotNull] string key, TextSpan keyTextSpan, int indent, bool isNested)
            {
                Key = key;
                KeyTextSpan = keyTextSpan;
                Indent = indent;
                IsNested = isNested;

                Value = string.Empty;
                ValueTextSpan = TextSpan.Empty;
            }

            public int Indent { get; }

            public bool IsNested { get; }

            [NotNull]
            public string Key { get; }

            public TextSpan KeyTextSpan { get; }

            [NotNull]
            public string Value { get; }

            public TextSpan ValueTextSpan { get; }
        }
    }
}
