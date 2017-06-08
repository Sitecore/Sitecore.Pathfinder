// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    [Export]
    public partial class YamlTextSnapshot : TextSnapshot
    {
        [CanBeNull]
        private ITextNode _root;

        [NotNull]
        private string _contents = string.Empty;

        [FactoryConstructor]
        [ImportingConstructor]
        public YamlTextSnapshot([NotNull] ISnapshotService snapshotService) : base(snapshotService)
        {
        }

        [NotNull]
        public virtual YamlTextSnapshot With([NotNull] ISourceFile sourceFile, [NotNull] string contents)
        {
            base.With(sourceFile);

            _contents = contents;

            return this;
        }

        public override ITextNode Root => _root ?? (_root = Parse(new Tokenizer(_contents)) ?? TextNode.Empty);

        [CanBeNull]
        protected ITextNode Parse([NotNull] Tokenizer tokenizer)
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
                    var childNode = Parse(tokenizer);
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
