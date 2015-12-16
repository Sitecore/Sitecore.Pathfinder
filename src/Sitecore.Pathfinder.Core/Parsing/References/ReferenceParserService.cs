// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Parsing.Pipelines.ReferenceParserPipelines;
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
        public ReferenceParserService([NotNull] IFactoryService factory, [NotNull] IPipelineService pipelines)
        {
            Factory = factory;
            Pipelines = pipelines;
        }

        [NotNull]
        protected IFactoryService Factory { get; }

        [NotNull]
        protected IPipelineService Pipelines { get; }

        public virtual IReference ParseReference(IProjectItem projectItem, ITextNode sourceTextNode, string referenceText)
        {
            var pipeline = Pipelines.Resolve<ReferenceParserPipeline>().Execute(Factory, projectItem, sourceTextNode, referenceText);
            return pipeline.Reference;
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

            // pipe seperated list
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

            // url string
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

            // plain text
            reference = ParseReference(projectItem, textNode, text);
            if (reference != null)
            {
                yield return reference;
            }
        }
    }
}
