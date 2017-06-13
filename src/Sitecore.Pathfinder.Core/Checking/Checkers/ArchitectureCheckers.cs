// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    [Export(typeof(IChecker)), Shared]
    public class ArchitectureCheckers : Checker
    {
        [ImportingConstructor]
        public ArchitectureCheckers([NotNull] IConfiguration configuration, [NotNull] IFileSystem fileSystem, [NotNull] ISnapshotService snapshotService, [NotNull] IFactory factory, [NotNull] IPathMapperService pathMapper)
        {
            Configuration = configuration;
            FileSystem = fileSystem;
            SnapshotService = snapshotService;
            Factory = factory;
            PathMapper = pathMapper;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFactory Factory { get; }

        [NotNull]
        protected IFileSystem FileSystem { get; }

        [NotNull]
        protected IPathMapperService PathMapper { get; }

        [NotNull]
        protected ISnapshotService SnapshotService { get; }

        [NotNull, ItemNotNull, Check]
        public IEnumerable<Diagnostic> ArchitectureSchema([NotNull] ICheckerContext context)
        {
            var roots = LoadSchemas(context).ToArray();

            foreach (var item in context.Project.Items)
            {
                foreach (var root in roots)
                {
                    foreach (var schemaTextNode in root.ChildNodes)
                    {
                        if (schemaTextNode.Key != "item")
                        {
                            continue;
                        }

                        if (!IsMatch(item, schemaTextNode))
                        {
                            continue;
                        }

                        foreach (var diagnostic in Match(item, schemaTextNode).ToArray())
                        {
                            yield return diagnostic;
                        }
                    }
                }
            }
        }

        [NotNull, ItemNotNull]
        protected virtual IEnumerable<ITextNode> GetSchemaChildNodes([NotNull] ITextNode schemaTextNode)
        {
            foreach (var childNode in schemaTextNode.ChildNodes)
            {
                switch (childNode.Key)
                {
                    case "item":
                        yield return childNode;

                        break;

                    case "element":
                        var refs = childNode.GetAttributeValue("ref");
                        if (string.IsNullOrEmpty(refs))
                        {
                            throw new InvalidOperationException($"'ref' attribute expected at {childNode.Snapshot.SourceFile.AbsoluteFileName} {childNode.TextSpan}");
                        }

                        foreach (var textNode in GetRefs(childNode, refs))
                        {
                            yield return textNode;
                        }

                        break;

                    default:
                        throw new InvalidOperationException($"Unexpected schema element '{childNode.Key} at {childNode.Snapshot.SourceFile.AbsoluteFileName} {childNode.TextSpan}");
                }
            }
        }

        [NotNull]
        protected virtual string GetText([NotNull] ITextNode textNode)
        {
            var texts = new List<string>();

            var itemName = textNode.GetAttributeValue("name");
            if (!string.IsNullOrEmpty(itemName))
            {
                texts.Add("named '" + itemName + "'");
            }

            var templateName = textNode.GetAttributeValue("template");
            if (!string.IsNullOrEmpty(templateName))
            {
                texts.Add("with template '" + templateName + "'");
            }

            var itemPath = textNode.GetAttributeValue("path");
            if (!string.IsNullOrEmpty(itemPath))
            {
                texts.Add("with path '" + itemPath + "'");
            }

            return string.Join(" and ", texts);
        }

        protected virtual bool IsMatch([NotNull] Item item, [NotNull] ITextNode textNode)
        {
            var isMatch = true;

            var itemName = textNode.GetAttributeValue("name");
            if (!string.IsNullOrEmpty(itemName))
            {
                if (!string.Equals(itemName, item.ItemName, StringComparison.OrdinalIgnoreCase))
                {
                    isMatch = false;
                }
            }

            var templateName = textNode.GetAttributeValue("template");
            if (!string.IsNullOrEmpty(templateName))
            {
                if (!string.Equals(templateName, item.TemplateName, StringComparison.OrdinalIgnoreCase))
                {
                    isMatch = false;
                }
            }

            var itemPath = textNode.GetAttributeValue("path");
            if (!string.IsNullOrEmpty(itemPath))
            {
                if (!string.Equals(itemPath, item.ItemIdOrPath, StringComparison.OrdinalIgnoreCase))
                {
                    isMatch = false;
                }
            }

            return isMatch;
        }

        [NotNull, ItemNotNull]
        protected virtual IEnumerable<ITextNode> LoadSchemas([NotNull] ICheckerContext context)
        {
            var pathMappingContext = new PathMappingContext(PathMapper);

            var directory = Path.Combine(Configuration.GetToolsDirectory(), "files\\architecture");

            foreach (var fileName in FileSystem.GetFiles(directory, "*.xml", SearchOption.AllDirectories))
            {
                var sourceFile = Factory.SourceFile(fileName);

                var snapshot = SnapshotService.LoadSnapshot(context.Project, sourceFile, pathMappingContext) as ITextSnapshot;
                if (snapshot != null)
                {
                    yield return snapshot.Root;
                }
            }
        }

        [NotNull, ItemNotNull]
        protected virtual IEnumerable<Diagnostic> Match([NotNull] Item item, [NotNull] ITextNode schemaTextNode)
        {
            var textNode = schemaTextNode;

            var reference = textNode.GetAttributeValue("ref");
            if (!string.IsNullOrEmpty(reference))
            {
                var refs = GetRefs(textNode, reference).ToArray();
                if (refs.Length > 1)
                {
                    throw new InvalidOperationException($"Referenced element may only contain one element at {textNode.Snapshot.SourceFile.AbsoluteFileName} {textNode.TextSpan}");
                }

                textNode = refs.First();
            }

            var fieldsTextNode = textNode.ChildNodes.FirstOrDefault(n => n.Key == "fields");
            if (fieldsTextNode != null)
            {
                foreach (var diagnostic in MatchFields(item, fieldsTextNode))
                {
                    yield return diagnostic;
                }
            }

            // check MinOccurs and MaxOccurs
            var childrenTextNode = textNode.ChildNodes.FirstOrDefault(n => n.Key == "children");
            if (childrenTextNode == null)
            {
                yield break;
            }

            var schemaChildNodes = GetSchemaChildNodes(childrenTextNode).ToArray();
            var children = item.Children.ToArray();

            foreach (var diagnostic in MatchMinMaxOccurs(item, schemaTextNode, schemaChildNodes, children))
            {
                yield return diagnostic;
            }

            foreach (var diagnostic in MatchUnexpectedItems(item, children, schemaChildNodes))
            {
                yield return diagnostic;
            }

            // descend into child items
            foreach (var child in children)
            {
                foreach (var schemaChildNode in schemaChildNodes)
                {
                    if (!IsMatch(child, schemaChildNode))
                    {
                        continue;
                    }

                    foreach (var diagnostic in Match(child, schemaChildNode))
                    {
                        yield return diagnostic;
                    }
                }
            }
        }

        [ItemNotNull, NotNull]
        protected virtual IEnumerable<Diagnostic> MatchUnexpectedItems([NotNull] Item item, [NotNull, ItemNotNull] Item[] children, [NotNull, ItemNotNull] ITextNode[] schemaChildNodes)
        {
            foreach (var child in children)
            {
                if (!schemaChildNodes.Any(c => IsMatch(child, c)))
                {
                    yield return new Diagnostic(Msg.D1025, item.Snapshot.SourceFile.RelativeFileName, TraceHelper.GetTextNode(child).TextSpan, Severity.Error, $"Unexpected item '{child.ItemIdOrPath}' [ArchitectureSchema]");
                }
            }
        }

        [NotNull, ItemNotNull]
        private IEnumerable<ITextNode> GetRefs([NotNull] ITextNode textNode, [NotNull] string reference)
        {
            var element = ((ITextSnapshot)textNode.Snapshot).Root.ChildNodes.FirstOrDefault(e => e.Key == "element" && e.GetAttributeValue("name") == reference);
            if (element == null)
            {
                throw new InvalidOperationException($"Element definition '{reference}' not found at {textNode.Snapshot.SourceFile.AbsoluteFileName} {textNode.TextSpan}");
            }

            foreach (var schemaChildNode in GetSchemaChildNodes(element))
            {
                yield return schemaChildNode;
            }
        }

        [ItemNotNull, NotNull]
        private IEnumerable<Diagnostic> MatchFields([NotNull] Item item, [NotNull] ITextNode fieldsTextNode)
        {
            foreach (var childNode in fieldsTextNode.ChildNodes)
            {
                var fieldName = childNode.GetAttributeValue("name");
                if (string.IsNullOrEmpty(fieldName))
                {
                    throw new InvalidOperationException($"Schema 'field' element must have 'name' attribute at {childNode.Snapshot.SourceFile.AbsoluteFileName} {childNode.TextSpan}");
                }

                var use = childNode.GetAttributeValue("use");

                var fieldValue = item[fieldName];

                if (use == "required" && string.IsNullOrEmpty(fieldValue))
                {
                    yield return new Diagnostic(Msg.C1065, item.Snapshot.SourceFile.RelativeFileName, TraceHelper.GetTextNode(item.Fields[fieldName], item).TextSpan, Severity.Error, $"Field '{fieldName}' is required [ArchitectureSchema]");
                }
            }
        }

        [ItemNotNull, NotNull]
        private IEnumerable<Diagnostic> MatchMinMaxOccurs([NotNull] Item item, [NotNull] ITextNode schemaTextNode, [NotNull, ItemNotNull] ITextNode[] schemaChildNodes, [NotNull, ItemNotNull] Item[] children)
        {
            foreach (var schemaChildNode in schemaChildNodes)
            {
                if (schemaChildNode.Key != "item")
                {
                    throw new InvalidOperationException($"Unexpected node {schemaChildNode.Key} at {schemaChildNode.Snapshot.SourceFile.AbsoluteFileName} {schemaChildNode.TextSpan}");
                }

                // in references, the referring text node may overwrite the referred text nodes

                var minOccurs = 0;
                var maxOccurs = 0;

                if (schemaChildNode.HasAttribute("minOccurs"))
                {
                    minOccurs = int.Parse(schemaChildNode.GetAttributeValue("minOccurs", "0"));
                }

                if (schemaTextNode.HasAttribute("minOccurs"))
                {
                    minOccurs = int.Parse(schemaTextNode.GetAttributeValue("minOccurs", "0"));
                }

                if (schemaChildNode.HasAttribute("maxOccurs"))
                {
                    maxOccurs = int.Parse(schemaChildNode.GetAttributeValue("maxOccurs", "0"));
                }

                if (schemaTextNode.HasAttribute("maxOccurs"))
                {
                    maxOccurs = int.Parse(schemaTextNode.GetAttributeValue("maxOccurs", "0"));
                }

                if (minOccurs <= 0 && maxOccurs <= 0)
                {
                    continue;
                }

                var count = children.Count(child => IsMatch(child, schemaChildNode));

                if (minOccurs > 0 && count < minOccurs)
                {
                    yield return new Diagnostic(Msg.D1025, item.Snapshot.SourceFile.RelativeFileName, TraceHelper.GetTextNode(item).TextSpan, Severity.Error, $"Item {GetText(schemaChildNode)} must occur at least {minOccurs} times [ArchitectureSchema]");
                }

                if (maxOccurs > 0 && count > maxOccurs)
                {
                    yield return new Diagnostic(Msg.D1025, item.Snapshot.SourceFile.RelativeFileName, TraceHelper.GetTextNode(item).TextSpan, Severity.Error, $"Item {GetText(schemaChildNode)} must not occur more than {maxOccurs} times [ArchitectureSchema]");
                }
            }
        }
    }
}
