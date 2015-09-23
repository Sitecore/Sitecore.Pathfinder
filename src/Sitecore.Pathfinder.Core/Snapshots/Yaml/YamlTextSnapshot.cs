// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots.Yaml
{
    public partial class YamlTextSnapshot : TextSnapshot
    {
        public YamlTextSnapshot([NotNull] ISourceFile sourceFile, [NotNull] string contents) : base(sourceFile)
        {
            var tokenizer = new Tokenizer(contents);

            Root = Parse(tokenizer, null) ?? TextNode.Empty;
        }

        public override ITextNode Root { get; }

        public override void SaveChanges()
        {
        }

        [CanBeNull]
        protected ITextNode Parse([NotNull] Tokenizer tokenizer, [CanBeNull] YamlTextNode parentTreeNode)
        {
            if (tokenizer.Token == null)
            {
                return null;
            }

            var textNode = new YamlTextNode(this, tokenizer.Token.KeyTextSpan, tokenizer.Token.Key, tokenizer.Token.Value, parentTreeNode);
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

                var attribute = new YamlTextNode(this, tokenizer.Token.KeyTextSpan, tokenizer.Token.Key, tokenizer.Token.Value, parentTreeNode);
                attributes.Add(attribute);
                tokenizer.Match();
            }

            return textNode;
        }
    }
}
