// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
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

        protected virtual void GetName([NotNull] IParseContext context, [NotNull] ITextNode sourceTextNode, [NotNull] out string name, [NotNull] out ITextNode nameTextNode, [NotNull] string elementName, [NotNull] string nameAttribute)
        {
            name = sourceTextNode.Key;
            if (!string.Equals(name, elementName, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(name))
            {
                nameTextNode = new AttributeNameTextNode(sourceTextNode);
                return;
            }

            name = sourceTextNode.Value;
            if (!string.IsNullOrEmpty(name))
            {
                nameTextNode = sourceTextNode;
                return;
            }

            var attr = sourceTextNode.GetAttribute(nameAttribute);
            if (attr != null)
            {
                name = attr.Value;
                nameTextNode = attr;
                return;
            }

            name = context.ItemName;
            nameTextNode = new FileNameTextNode(context.ItemName, sourceTextNode.Snapshot);
        }
    }
}
