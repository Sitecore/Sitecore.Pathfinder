// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.References;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
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
        protected virtual Attribute<string> GetItemName([NotNull] IParseContext context, [NotNull] ITextNode textNode, [NotNull] string attributeName = "Name")
        {
            var itemNameTextNode = textNode.GetAttributeTextNode(attributeName);

            string value;
            ITextNode source;

            if (itemNameTextNode != null)
            {
                value = itemNameTextNode.Value;
                source = itemNameTextNode;
            }
            else
            {
                value = context.ItemName;
                source = new FileNameTextNode(value, textNode.Snapshot);
            }

            var attribute = new Attribute<string>(attributeName, value);
            attribute.AddSource(source);

            return attribute;
        }

        [CanBeNull]
        protected virtual IReference ParseReference([NotNull] ItemParseContext context, [NotNull] IProjectItem projectItem, [NotNull] ITextNode source, [NotNull] string text)
        {
            if (text.StartsWith("/sitecore/", StringComparison.OrdinalIgnoreCase))
            {
                var sourceAttribute = new Attribute<string>(source.Name, string.Empty);
                sourceAttribute.AddSource(source);
                sourceAttribute.SourceFlags = SourceFlags.IsQualified;
                return context.ParseContext.Factory.Reference(projectItem, sourceAttribute, text);
            }

            Guid guid;
            if (Guid.TryParse(text, out guid))
            {
                var sourceAttribute = new Attribute<string>(source.Name, string.Empty);
                sourceAttribute.AddSource(source);
                sourceAttribute.SourceFlags = SourceFlags.IsGuid;
                return context.ParseContext.Factory.Reference(projectItem, sourceAttribute, guid.ToString("B").ToUpperInvariant());
            }

            return null;
        }

        [NotNull]
        protected virtual IEnumerable<IReference> ParseReferences([NotNull] ItemParseContext context, [NotNull] IProjectItem projectItem, [NotNull] ITextNode source, [NotNull] string text)
        {
            var reference = ParseReference(context, projectItem, source, text);
            if (reference != null)
            {
                yield return reference;
                yield break;
            }

            if (text.IndexOf('|') >= 0)
            {
                var parts = text.Split(Constants.Pipe, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    reference = ParseReference(context, projectItem, source, part);
                    if (reference != null)
                    {
                        yield return reference;
                    }
                }

                yield break;
            }

            if (text.IndexOf('&') < 0 && text.IndexOf('=') < 0)
            {
                yield break;
            }

            var urlString = new UrlString(text);

            foreach (string key in urlString.Parameters)
            {
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                var value = urlString.Parameters[key];

                reference = ParseReference(context, projectItem, source, value);
                if (reference != null)
                {
                    yield return reference;
                }
            }
        }
    }
}
