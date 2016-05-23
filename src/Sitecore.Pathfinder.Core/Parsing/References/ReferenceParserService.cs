// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Parsing.Pipelines.ReferenceParserPipelines;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.References;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing.References
{
    [Export(typeof(IReferenceParserService))]
    public class ReferenceParserService : IReferenceParserService
    {
        [ImportingConstructor]
        public ReferenceParserService([NotNull] IConfiguration configuration, [NotNull] IFactoryService factory, [NotNull] IPipelineService pipelines)
        {
            Configuration = configuration;
            Factory = factory;
            Pipelines = pipelines;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFactoryService Factory { get; }

        [NotNull]
        protected IPipelineService Pipelines { get; }

        public virtual bool IsIgnoredReference(string referenceText)
        {
            // todo: cache this
            foreach (var pair in Configuration.GetSubKeys(Constants.Configuration.CheckProjectIgnoredReferences))
            {
                var op = Configuration.Get(Constants.Configuration.CheckProjectIgnoredReferences + ":" + pair.Key);
                switch (op)
                {
                    case "starts-with":
                        if (referenceText.StartsWith(pair.Key, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }

                        break;
                    case "ends-with":
                        if (referenceText.EndsWith(pair.Key, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }

                        break;
                    case "contains":
                        if (referenceText.IndexOf(pair.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            return true;
                        }

                        break;

                    default:
                        if (string.Equals(referenceText, pair.Key, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }

                        break;
                }
            }

            return false;
        }

        public virtual IReference ParseReference(IProjectItem projectItem, ITextNode sourceTextNode, string referenceText)
        {
            if (IsIgnoredReference(referenceText))
            {
                return null;
            }

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

        public virtual IEnumerable<IReference> ParseReferences(IProjectItem projectItem, ITextNode textNode)
        {
            var referenceText = textNode.Value.Trim();

            // query string: ignore
            if (referenceText.StartsWith("query:"))
            {
                yield break;
            }

            // todo: process media links 
            if (referenceText.StartsWith("/~/media") || referenceText.StartsWith("~/media"))
            {
                yield break;
            }

            // todo: process icon links 
            if (referenceText.StartsWith("/~/icon") || referenceText.StartsWith("~/icon"))
            {
                yield break;
            }

            foreach (var reference in ParseItemPaths(projectItem, textNode, referenceText))
            {
                yield return reference;
            }

            foreach (var reference in ParseGuids(projectItem, textNode, referenceText))
            {
                yield return reference;
            }

            foreach (var reference in ParseFilePaths(projectItem, textNode, referenceText))
            {
                yield return reference;
            }
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

        protected virtual int EndOfItemPath([NotNull] string text, int start)
        {
            var chars = text.ToCharArray();

            for (var i = start; i < text.Length; i++)
            {
                var c = chars[i];
                if (!char.IsDigit(c) && !char.IsLetter(c) && c != '/' && c != ' ' && c != '-' && c != '.' && c != '_' && c != '~')
                {
                    return i;
                }
            }

            return text.Length;
        }

        [ItemNotNull, NotNull]
        protected virtual IEnumerable<IReference> ParseFilePaths([NotNull] IProjectItem projectItem, [NotNull] ITextNode textNode, [NotNull] string referenceText)
        {
            var s = 0;
            while (true)
            {
                var n = referenceText.IndexOf("~/", s, StringComparison.Ordinal);
                if (n < 0)
                {
                    break;
                }

                var e = EndOfFilePath(referenceText, n);
                var text = referenceText.Mid(n, e - n);

                var reference = ParseReference(projectItem, textNode, text);
                if (reference != null)
                {
                    yield return reference;
                }

                s = e;
            }
        }

        [ItemNotNull, NotNull]
        protected virtual IEnumerable<IReference> ParseGuids([NotNull] IProjectItem projectItem, [NotNull] ITextNode textNode, [NotNull] string referenceText)
        {
            var s = 0;
            while (true)
            {
                var n = referenceText.IndexOf('{', s);
                if (n < 0)
                {
                    break;
                }

                // ignore uids
                if (n > 4 && referenceText.Mid(n - 5, 5) == "uid=\"")
                {
                    s = n + 1;
                    continue;
                }

                var e = referenceText.IndexOf('}', n);
                if (e < 0)
                {
                    break;
                }

                e++;

                var text = referenceText.Mid(n, e - n);

                var reference = ParseReference(projectItem, textNode, text);
                if (reference != null)
                {
                    yield return reference;
                }

                s = e;
            }
        }

        [ItemNotNull, NotNull]
        protected virtual IEnumerable<IReference> ParseItemPaths([NotNull] IProjectItem projectItem, [NotNull] ITextNode textNode, [NotNull] string referenceText)
        {
            var s = 0;
            while (true)
            {
                var n = referenceText.IndexOf("/sitecore", s, StringComparison.OrdinalIgnoreCase);
                if (n < 0)
                {
                    break;
                }

                var e = EndOfItemPath(referenceText, n);
                var text = referenceText.Mid(n, e - n);

                var reference = ParseReference(projectItem, textNode, text);
                if (reference != null)
                {
                    yield return reference;
                }

                s = e;
            }
        }
    }
}
