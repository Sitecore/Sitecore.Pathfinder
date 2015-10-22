// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Snapshots
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TextSnapshot : Snapshot, ITextSnapshot
    {
        [ImportingConstructor]
        public TextSnapshot([NotNull] ISnapshotService snapshotService)
        {
            SnapshotService = snapshotService;
        }

        [NotNull]
        protected ISnapshotService SnapshotService { get; }

        public string ParseError { get; protected set; } = string.Empty;

        public TextSpan ParseErrorTextSpan { get; protected set; } = TextSpan.Empty;

        public virtual ITextNode Root { get; protected set; }

        public virtual void ValidateSchema(IParseContext context)
        {
        }

        public override ISnapshot With(ISourceFile sourceFile)
        {
            base.With(sourceFile);

            Root = new SnapshotTextNode(this);

            return this;
        }

        [NotNull]
        protected virtual ITextNode ParseDirectives([NotNull] SnapshotParseContext parseContext, [NotNull] ITextNode textNode)
        {
            var childNodes = (List<ITextNode>)textNode.ChildNodes;

            for (var index = childNodes.Count - 1; index >= 0; index--)
            {
                ParseDirectives(parseContext, textNode, childNodes[index]);
            }

            return textNode;
        }

        protected virtual void ParseDirectives([NotNull] SnapshotParseContext parseContext, [NotNull] ITextNode parentTextNode, [NotNull] ITextNode textNode)
        {
            IEnumerable<ITextNode> newTextNodes = null;

            switch (textNode.Key)
            {
                case "Include":
                    newTextNodes = ParseIncludeDirective(parseContext, textNode);
                    break;
            }

            if (newTextNodes == null)
            {
                ParseDirectives(parseContext, textNode);
                return;
            }

            var childNodes = (List<ITextNode>)parentTextNode.ChildNodes;
            var index = childNodes.IndexOf(textNode);
            childNodes.Remove(textNode);
            childNodes.InsertRange(index, newTextNodes);

            foreach (var newTextNode in newTextNodes)
            {
                ParseDirectives(parseContext, newTextNode);
            }
        }

        [NotNull]
        [ItemNotNull]
        protected virtual IEnumerable<ITextNode> ParseIncludeDirective([NotNull] SnapshotParseContext parseContext, [NotNull] ITextNode textNode)
        {
            var textNodes = new List<ITextNode>();

            if (!textNode.Attributes.Any() && textNode.ChildNodes.Any())
            {
                foreach (var childNode in textNode.ChildNodes)
                {
                    textNodes.AddRange(ParseIncludeDirective(parseContext, childNode));
                }

                return textNodes;
            }

            var fileName = textNode.GetAttributeValue("File");
            if (string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(textNode.Value))
            {
                fileName = textNode.Value;
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new InvalidOperationException("'File' attribute expected");
            }

            var innerTextNodes = new Dictionary<string, ITextNode>(parseContext.InnerTextNodes);
            
            var nonPlaceholderTextNodes = textNode.ChildNodes.Where(e => e.Key != "Placeholder").ToList();
            if (nonPlaceholderTextNodes.Any())
            {
                var parentTextNode = new TextNode(this, string.Empty, string.Empty, TextSpan.Empty);

                var childNodes = (List<ITextNode>)parentTextNode.ChildNodes;
                foreach (var e in nonPlaceholderTextNodes)
                {
                    childNodes.Add(e);
                }

                innerTextNodes[string.Empty] = parentTextNode;
            }

            foreach (var source in textNode.ChildNodes.Where(e => e.Key == "Placeholder"))
            {
                var key = source.GetAttributeValue("Key");
                var parentTextNode = new TextNode(this, string.Empty, string.Empty, TextSpan.Empty);

                var childNodes = (List<ITextNode>)parentTextNode.ChildNodes;
                foreach (var e in nonPlaceholderTextNodes)
                {
                    childNodes.Add(e);
                }

                innerTextNodes[key] = parentTextNode;
            }

            ((List<ITextNode>)(textNode.ChildNodes)).Clear();
            
            var tokens = new Dictionary<string, string>(parseContext.Tokens).AddRange(textNode.Attributes.ToDictionary(a => a.Key, a => a.Value));
            var context = new SnapshotParseContext(tokens, innerTextNodes);

            textNodes.Add(SnapshotService.LoadIncludeFile(this, fileName, context));

            return textNodes;
        }
    }
}
