// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.References;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Parsing.References
{
    [Export(typeof(IReferenceParserService))]
    public class ReferenceParserService : IReferenceParserService
    {
        [ImportingConstructor]
        public ReferenceParserService([NotNull] IFactoryService factory)
        {
            Factory = factory;
        }

        [NotNull]
        protected IFactoryService Factory { get; }

        public virtual IReference ParseReference(IProjectItem projectItem, ITextNode sourceTextNode, string referenceText)
        {
            if (referenceText.StartsWith("/sitecore/", StringComparison.OrdinalIgnoreCase))
            {
                var sourceProperty = new SourceProperty<string>(sourceTextNode.Key, string.Empty, SourcePropertyFlags.IsQualified);
                sourceProperty.SetValue(sourceTextNode);
                return Factory.Reference(projectItem, sourceProperty, referenceText);
            }

            Guid guid;
            if (Guid.TryParse(referenceText, out guid))
            {
                var sourceProperty = new SourceProperty<string>(sourceTextNode.Key, string.Empty, SourcePropertyFlags.IsGuid);
                sourceProperty.SetValue(sourceTextNode);
                return Factory.Reference(projectItem, sourceProperty, referenceText);
            }

            if (referenceText.StartsWith("{") && referenceText.EndsWith("}"))
            {
                var sourceProperty = new SourceProperty<string>(sourceTextNode.Key, string.Empty, SourcePropertyFlags.IsSoftGuid);
                sourceProperty.SetValue(sourceTextNode);
                return Factory.Reference(projectItem, sourceProperty, referenceText);
            }

            return null;
        }

        [ItemNotNull]
        public virtual IEnumerable<IReference> ParseReferences<T>(IProjectItem projectItem, SourceProperty<T> sourceProperty)
        {
            var sourceTextNode = sourceProperty.SourceTextNode;
            if (sourceTextNode == null)
            {
                return Enumerable.Empty<IReference>();
            }

            return ParseReferences(projectItem, sourceTextNode);
        }

        [ItemNotNull]
        public virtual IEnumerable<IReference> ParseReferences(IProjectItem projectItem, ITextNode textNode)
        {
            var text = textNode.Value;

            IReference reference;
            if (text.IndexOf('|') >= 0)
            {
                var parts = text.Split(Constants.Pipe, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    reference = ParseReference(projectItem, textNode, part);
                    if (reference != null)
                    {
                        yield return reference;
                    }
                }

                yield break;
            }

            if (text.IndexOf('&') >= 0 || text.IndexOf('=') >= 0)
            {
                var urlString = new UrlString(text);

                foreach (string key in urlString.Parameters)
                {
                    if (string.IsNullOrEmpty(key))
                    {
                        continue;
                    }

                    var parameterValue = urlString.Parameters[key];

                    reference = ParseReference(projectItem, textNode, parameterValue);
                    if (reference != null)
                    {
                        yield return reference;
                    }
                }

                yield break;
            }

            reference = ParseReference(projectItem, textNode, text);
            if (reference != null)
            {
                yield return reference;
            }
        }
    }
}
