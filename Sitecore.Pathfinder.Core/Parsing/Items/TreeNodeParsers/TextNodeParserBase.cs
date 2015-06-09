// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.References;
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

        [CanBeNull]
        protected virtual IReference ParseReference([NotNull] ItemParseContext context, [NotNull] IProjectItem projectItem, [NotNull] ITextNode source, [NotNull] string text)
        {
            if (text.StartsWith("/sitecore/", StringComparison.OrdinalIgnoreCase))
            {
                return context.ParseContext.Factory.Reference(projectItem, source, text);
            }

            Guid guid;
            if (Guid.TryParse(text, out guid))
            {
                return context.ParseContext.Factory.Reference(projectItem, source, guid.ToString("B").ToUpperInvariant());
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
