// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class YamlTextSnapshot : TextSnapshot
    {
        [ImportingConstructor]
        public YamlTextSnapshot([NotNull] ISnapshotService snapshotService)
        {
            SnapshotService = snapshotService;
        }

        [NotNull]
        protected ISnapshotService SnapshotService { get; }

        [NotNull]
        protected IDictionary<string, string> Tokens { get; private set; }

        public override void SaveChanges()
        {
        }

        [NotNull]
        public virtual YamlTextSnapshot With([NotNull] ISourceFile sourceFile, [NotNull] string contents, [NotNull] IDictionary<string, string> tokens)
        {
            base.With(sourceFile);

            Tokens = tokens;

            var tokenizer = new Tokenizer(contents);
            Root = Parse(tokenizer, null) ?? TextNode.Empty;

            return this;
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
                    if (tokenizer.Token.Key == "Include")
                    {
                        ParseIncludeFile(tokenizer, childNodes);
                        continue;
                    }

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

        [NotNull]
        protected virtual ITextNode ParseIncludeFile([NotNull] Tokenizer tokenizer, [NotNull] [ItemNotNull] ICollection<ITextNode> childNodes)
        {
            if (tokenizer.Token == null)
            {
                return TextNode.Empty;
            }

            var fileName = tokenizer.Token.Value;
            if (string.IsNullOrEmpty(fileName))
            {
                throw new InvalidOperationException("'File' attribute expected");
            }

            var startIndent = tokenizer.Token.Indent;

            tokenizer.Match();
            if (tokenizer.Token == null)
            {
                return TextNode.Empty;
            }

            var attributes = new Dictionary<string, string>();
            while (tokenizer.Token != null && tokenizer.Token.Indent > startIndent)
            {
                if (!tokenizer.Token.IsNested)
                {
                    attributes[tokenizer.Token.Key] = tokenizer.Token.Value;
                }

                tokenizer.Match();
            }

            var tokens = new Dictionary<string, string>(Tokens).AddRange(attributes.ToDictionary(a => a.Key, a => a.Value));

            var textNode = SnapshotService.LoadIncludeFile(this, fileName, tokens);
            if (textNode != TextNode.Empty)
            {
                childNodes.Add(textNode);
            }

            return textNode;
        }
    }
}
