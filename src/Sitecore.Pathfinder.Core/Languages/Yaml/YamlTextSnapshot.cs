// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    [Export]
    public partial class YamlTextSnapshot : TextSnapshot
    {
        [ImportingConstructor]
        public YamlTextSnapshot([NotNull] ISnapshotService snapshotService) : base(snapshotService)
        {
        }

        [NotNull]
        protected SnapshotParseContext ParseContext { get; private set; }

        [NotNull]
        public virtual YamlTextSnapshot With([NotNull] SnapshotParseContext parseContext, [NotNull] ISourceFile sourceFile, [NotNull] string contents)
        {
            base.With(sourceFile);

            ParseContext = parseContext;

            var tokenizer = new Tokenizer(contents);
            Root = Parse(tokenizer, null) ?? TextNode.Empty;

            if (Root != null)
            {
                Root = ParseDirectives(ParseContext, Root);
            }

            return this;
        }

        [CanBeNull]
        protected ITextNode Parse([NotNull] Tokenizer tokenizer, [CanBeNull] YamlTextNode parentTreeNode)
        {
            if (tokenizer.Token == null)
            {
                return null;
            }

            var textNode = new YamlTextNode(this, tokenizer.Token.KeyTextSpan, tokenizer.Token.Key, tokenizer.Token.Value);

            var attributes = (ICollection<ITextNode>)textNode.Attributes;
            var childNodes = (ICollection<ITextNode>)textNode.ChildNodes;

            var startIndent = tokenizer.Token.Indent;

            tokenizer.Match();

            while (tokenizer.Token != null && tokenizer.Token.Indent > startIndent)
            {
                if (tokenizer.Token.IsNested)
                {
                    var childNode = Parse(tokenizer, textNode);
                    if (childNode != null)
                    {
                        childNodes.Add(childNode);
                    }

                    continue;
                }

                var attribute = new YamlTextNode(this, tokenizer.Token.KeyTextSpan, tokenizer.Token.Key, tokenizer.Token.Value);
                attributes.Add(attribute);
                tokenizer.Match();
            }

            return textNode;
        }
    }
}
