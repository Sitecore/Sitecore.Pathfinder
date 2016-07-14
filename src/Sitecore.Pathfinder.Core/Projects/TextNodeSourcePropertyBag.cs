// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects
{
    public abstract class TextNodeSourcePropertyBag : SourcePropertyBag, IHasSourceTextNodes
    {
        [NotNull, ItemNotNull]
        private readonly IList<ITextNode> _sourceTextNodes;

        protected TextNodeSourcePropertyBag()
        {
            _sourceTextNodes = new LockableList<ITextNode>(this);
        }

        public IEnumerable<ITextNode> AdditionalSourceTextNodes => _sourceTextNodes.Skip(1);

        public ITextNode SourceTextNode => _sourceTextNodes.FirstOrDefault() ?? TextNode.Empty;

        protected void Merge([NotNull] TextNodeSourcePropertyBag other, bool overwrite)
        {
            _sourceTextNodes.Merge(SourceTextNode, other.AdditionalSourceTextNodes, other.SourceTextNode, overwrite);
        }

        [NotNull]
        protected TextNodeSourcePropertyBag WithAdditionalSourceTextNode([NotNull] ITextNode textNode)
        {
            if (!_sourceTextNodes.Contains(textNode))
            {
                _sourceTextNodes.Add(textNode);
            }

            return this;
        }

        [NotNull]
        protected TextNodeSourcePropertyBag WithSourceTextNode([NotNull] ITextNode textNode)
        {
            _sourceTextNodes.Remove(textNode);
            _sourceTextNodes.Insert(0, textNode);

            return this;
        }
    }
}
