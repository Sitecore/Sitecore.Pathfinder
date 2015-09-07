// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
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
        protected virtual ITextNode GetItemNameTextNode([NotNull] IParseContext context, [NotNull] ITextNode textNode, [NotNull] string attributeName = "Name")
        {
            var itemNameTextNode = textNode.GetAttributeTextNode(attributeName);

            var source = itemNameTextNode ?? new FileNameTextNode(context.ItemName, textNode.Snapshot);

            return source;
        }

        [CanBeNull]
        protected virtual IReference ParseReference([NotNull] ItemParseContext context, [NotNull] IProjectItem projectItem, [NotNull] ITextNode source, [NotNull] string text)
        {
            if (text.StartsWith("/sitecore/", StringComparison.OrdinalIgnoreCase))
            {
                var sourceProperty = new SourceProperty<string>(source.Name, string.Empty, SourcePropertyFlags.IsQualified);
                sourceProperty.SetValue(source);
                return context.ParseContext.Factory.Reference(projectItem, sourceProperty, text);
            }

            Guid guid;
            if (Guid.TryParse(text, out guid))
            {
                var sourceProperty = new SourceProperty<string>(source.Name, string.Empty, SourcePropertyFlags.IsGuid);
                sourceProperty.SetValue(source);
                return context.ParseContext.Factory.Reference(projectItem, sourceProperty, guid.Format());
            }

            return null;
        }

        [NotNull]
        protected virtual IEnumerable<IReference> ParseReferences([NotNull] ItemParseContext context, [NotNull] IProjectItem projectItem, [NotNull] ITextNode textNode, [NotNull] string text)
        {
            var reference = ParseReference(context, projectItem, textNode, text);
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
                    reference = ParseReference(context, projectItem, textNode, part);
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

                reference = ParseReference(context, projectItem, textNode, value);
                if (reference != null)
                {
                    yield return reference;
                }
            }
        }
    }
}
