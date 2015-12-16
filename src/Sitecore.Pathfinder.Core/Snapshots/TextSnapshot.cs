// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Snapshots
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class TextSnapshot : Snapshot, ITextSnapshot
    {
        [ImportingConstructor]
        public TextSnapshot([NotNull] ISnapshotService snapshotService)
        {
            SnapshotService = snapshotService;
        }

        public string ParseError { get; protected set; } = string.Empty;

        public TextSpan ParseErrorTextSpan { get; protected set; } = TextSpan.Empty;

        public virtual ITextNode Root { get; protected set; }

        [NotNull]
        protected ISnapshotService SnapshotService { get; }

        public virtual bool ValidateSchema(IParseContext context)
        {
            return true;
        }

        public override ISnapshot With(ISourceFile sourceFile)
        {
            base.With(sourceFile);

            Root = new SnapshotTextNode(this);

            return this;
        }

        [NotNull]
        protected virtual ITextNode ParseDirectives([NotNull] SnapshotParseContext snapshotParseContext, [NotNull] ITextNode textNode)
        {
            // todo: dangereous cast
            var childNodes = (List<ITextNode>)textNode.ChildNodes;

            for (var index = childNodes.Count - 1; index >= 0; index--)
            {
                ParseDirectives(snapshotParseContext, textNode, childNodes[index]);
            }

            return textNode;
        }

        protected virtual void ParseDirectives([NotNull] SnapshotParseContext snapshotParseContext, [NotNull] ITextNode parentTextNode, [NotNull] ITextNode textNode)
        {
            var mutableParentTextNode = parentTextNode as IMutableTextNode;
            Assert.Cast(mutableParentTextNode, nameof(mutableParentTextNode));

            IEnumerable<ITextNode> newTextNodes = null;

            foreach (var directive in SnapshotService.Directives)
            {
                if (!directive.CanParse(textNode))
                {
                    continue;
                }

                newTextNodes = directive.Parse(snapshotParseContext, textNode);
                break;
            }

            if (newTextNodes == null)
            {
                ParseDirectives(snapshotParseContext, textNode);
                return;
            }


            var childNodes = mutableParentTextNode.ChildNodeCollection;
            var index = childNodes.IndexOf(textNode);
            childNodes.Remove(textNode);
            
            // todo: remove direct cast
            ((List<ITextNode>)childNodes).InsertRange(index, newTextNodes);

            foreach (var newTextNode in newTextNodes)
            {
                ParseDirectives(snapshotParseContext, newTextNode);
            }
        }
    }
}
