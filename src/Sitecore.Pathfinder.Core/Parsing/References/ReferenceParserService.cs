// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
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

        public virtual IEnumerable<IReference> ParseReferences<T>(IProjectItem projectItem, SourceProperty<T> sourceProperty)
        {
            var sourceTextNode = sourceProperty.SourceTextNode;
            if (sourceTextNode == null)
            {
                return Enumerable.Empty<IReference>();
            }

            return ParseReferences(projectItem, sourceTextNode);
        }

        protected virtual int EndOfItemPath([NotNull] string text, int start)
        {
            var chars = text.ToCharArray();

            for (var i = start; i < text.Length; i++)
            {
                var c = chars[i];
                if (!char.IsDigit(c) && !char.IsLetter(c) && c != '/' && c != ' ' && c != '-' && c != '.')
                {
                    return i;
                }
            }

            return text.Length;
        }

        protected virtual int EndOfFilePath([NotNull] string text, int start)
        {
            var chars = text.ToCharArray();
            var invalidChars = Path.GetInvalidPathChars();

            for (var i = start; i < text.Length; i++)
            {
                var c = chars[i];
                if (invalidChars.Contains(c))
                {
                    return i;
                }
            }

            return text.Length;
        }

        public virtual IEnumerable<IReference> ParseReferences(IProjectItem projectItem, ITextNode textNode)
        {
            var text = textNode.Value.Trim();

            // query string: ignore
            if (text.StartsWith("query:"))
            {
                yield break;
            }

            // todo: process media links 
            if (text.StartsWith("/~/media") || text.StartsWith("~/media"))
            {
                yield break;
            }

            // todo: process icon links 
            if (text.StartsWith("/~/icon") || text.StartsWith("~/icon"))
            {
                yield break;
            }

            // look for item paths
            var s = 0;
            while (true)
            {
                var n = text.IndexOf("/sitecore", s, StringComparison.OrdinalIgnoreCase);
                if (n < 0)
                {
                    break;
                }

                var e = EndOfItemPath(text, n);
                var referenceText = text.Mid(n, e - n);

                var reference = ParseReference(projectItem, textNode, referenceText);
                if (reference != null)
                {
                    yield return reference;
                }

                s = e;
            }

            // look for guids and soft guids
            s = 0;
            while (true)
            {
                var n = text.IndexOf('{', s);
                if (n < 0)
                {
                    break;
                }

                // ignore uids
                if (n > 4 && text.Mid(n - 5, 5) == "uid=\"")
                {
                    s = n + 1;
                    continue;
                }

                var e = text.IndexOf('}', n);
                if (e < 0)
                {
                    break;
                }

                e++;

                var referenceText = text.Mid(n, e - n);

                var reference = ParseReference(projectItem, textNode, referenceText);
                if (reference != null)
                {
                    yield return reference;
                }

                s = e;
            }
            
            // look for file paths
            s = 0;
            while (true)
            {
                var n = text.IndexOf("~/", s, StringComparison.Ordinal);
                if (n < 0)
                {
                    break;
                }

                var e = EndOfFilePath(text, n);
                var referenceText = text.Mid(n, e - n);

                var reference = ParseReference(projectItem, textNode, referenceText);
                if (reference != null)
                {
                    yield return reference;
                }

                s = e;
            }
            
            /*
            // pipe seperated list of guids
            if (text.StartsWith("{") && text.EndsWith("}") && text.IndexOf('|') >= 0 && text.IndexOf('"') < 0)
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
            */
        }
    }
}
