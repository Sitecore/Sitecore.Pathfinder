// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing.Items
{
    public abstract class TextNodeParserBase : ITextNodeParser
    {
        protected TextNodeParserBase(double priority)
        {
            Priority = priority;
        }

        public double Priority { get; }

        public abstract bool CanParse(ItemParseContext context, ITextNode textNode);

        public abstract void Parse(ItemParseContext context, ITextNode textNode);

        [NotNull]
        protected virtual ITextNode GetItemNameTextNode([NotNull] IParseContext context, [NotNull] ITextNode textNode, [NotNull] string attributeName = "Name")
        {
            return textNode.GetAttribute(attributeName) ?? new FileNameTextNode(context.ItemName, textNode.Snapshot);
        }
    }
}
