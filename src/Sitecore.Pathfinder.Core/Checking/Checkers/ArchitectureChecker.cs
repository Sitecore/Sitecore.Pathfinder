// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    public class ArchitectureChecker : Checker
    {
        [ImportingConstructor]
        public ArchitectureChecker([NotNull] IFileSystemService fileSystem, [NotNull] ISnapshotService snapshotService, [NotNull] IFactoryService factory, [NotNull] IPathMapperService pathMapper)
        {
            FileSystem = fileSystem;
            SnapshotService = snapshotService;
            Factory = factory;
            PathMapper = pathMapper;
        }

        [NotNull]
        protected IFactoryService Factory { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        [NotNull]
        protected IPathMapperService PathMapper { get; }

        [NotNull]
        protected ISnapshotService SnapshotService { get; }

        [NotNull, ItemNotNull, Export("Check")]
        public IEnumerable<Diagnostic> ArchitectureSchema([NotNull] ICheckerContext context)
        {
            var roots = LoadSchemas(context).ToArray();

            foreach (var item in context.Project.Items)
            {
                foreach (var root in roots)
                {
                    foreach (var schemaTextNode in root.ChildNodes)
                    {
                        if (schemaTextNode.Key != "Item")
                        {
                            continue;
                        }

                        if (!IsMatch(item, schemaTextNode))
                        {
                            continue;
                        }

                        foreach (var diagnostic in Match(item, schemaTextNode))
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
                    case "Item":
                        yield return childNode;
                        break;

                    case "Element":
                        foreach (var textNode in GetRefs(childNode, childNode.GetAttributeValue("Ref")))
                        {
                            yield return textNode;
                        }

                        break;

                    default:
                        throw new InvalidOperationException("Unexpected schema element: " + childNode.Key);
                }
            }
        }

        [NotNull, ItemNotNull]
        private IEnumerable<ITextNode> GetRefs([NotNull] ITextNode textNode, [NotNull] string reference)
        {
            var elements = ((ITextSnapshot)textNode.Snapshot).Root.ChildNodes.FirstOrDefault(c => c.Key == "Elements");
            if (elements == null)
            {
                throw new InvalidOperationException("Element definition not found: " + reference);
            }

            var element = elements.ChildNodes.FirstOrDefault(e => e.Key == "Element" && e.GetAttributeValue("Name") == reference);
            if (element == null)
            {
                throw new InvalidOperationException("Element definition not found: " + reference);
            }

            foreach (var schemaChildNode in GetSchemaChildNodes(element))
            {
                yield return schemaChildNode;
            }
        }

        [NotNull]
        protected virtual string GetText([NotNull] ITextNode textNode)
        {
            var texts = new List<string>();

            var itemName = textNode.GetAttributeValue("Name");
            if (!string.IsNullOrEmpty(itemName))
            {
                texts.Add("named '" + itemName + "'");
            }

            var templateName = textNode.GetAttributeValue("Template");
            if (!string.IsNullOrEmpty(templateName))
            {
                texts.Add("with template '" + templateName + "'");
            }

            var itemPath = textNode.GetAttributeValue("Path");
            if (!string.IsNullOrEmpty(itemPath))
            {
                texts.Add("with path '" + itemPath + "'");
            }

            return string.Join(" and ", texts);
        }

        protected virtual bool IsMatch([NotNull] Item item, [NotNull] ITextNode textNode)
        {
            var isMatch = true;

            var itemName = textNode.GetAttributeValue("Name");
            if (!string.IsNullOrEmpty(itemName))
            {
                if (itemName != item.ItemName)
                {
                    isMatch = false;
                }
            }

            var templateName = textNode.GetAttributeValue("Template");
            if (!string.IsNullOrEmpty(templateName))
            {
                if (templateName != item.TemplateName)
                {
                    isMatch = false;
                }
            }

            var itemPath = textNode.GetAttributeValue("Path");
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

            var directory = Path.Combine(context.Configuration.GetToolsDirectory(), "files\\architecture");

            foreach (var fileName in FileSystem.GetFiles(directory, "*.xml", SearchOption.AllDirectories))
            {
                var sourceFile = Factory.SourceFile(FileSystem, fileName);

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

            var reference = textNode.GetAttributeValue("Ref");
            if (!string.IsNullOrEmpty(reference))
            {
                var refs = GetRefs(textNode, reference).ToArray();
                if (refs.Length > 1)
                {
                     throw new InvalidOperationException("Referenced element may only contain one element");
                }

                textNode = refs.First();
            }

            // check MinOccurs and MaxOccurs
            var childrenTextNode = textNode.ChildNodes.FirstOrDefault(n => n.Key == "Children");
            if (childrenTextNode == null)
            {
                yield break;
            }

            var schemaChildNodes = GetSchemaChildNodes(childrenTextNode).ToArray();
            var children = item.Children.ToArray();

            foreach (var schemaChildNode in schemaChildNodes)
            {
                if (schemaChildNode.Key != "Item")
                {
                    throw new InvalidOperationException("Unexpected node: " + schemaChildNode.Key);
                }

                // in references, the referring text node may overwrite the referred text nodes

                var minOccurs = 0;
                var maxOccurs = 0;

                if (schemaChildNode.HasAttribute("MinOccurs"))
                {
                    minOccurs = int.Parse(schemaChildNode.GetAttributeValue("MinOccurs", "0"));
                }

                if (schemaTextNode.HasAttribute("MinOccurs"))
                {
                    minOccurs = int.Parse(schemaTextNode.GetAttributeValue("MinOccurs", "0"));
                }

                if (schemaChildNode.HasAttribute("MaxOccurs"))
                {
                    maxOccurs = int.Parse(schemaChildNode.GetAttributeValue("MaxOccurs", "0"));
                }

                if (schemaTextNode.HasAttribute("MaxOccurs"))
                {
                    maxOccurs = int.Parse(schemaTextNode.GetAttributeValue("MaxOccurs", "0"));
                }

                if (minOccurs <= 0 && maxOccurs <= 0)
                {
                    continue;
                }

                var count = children.Count(child => IsMatch(child, schemaChildNode));

                if (minOccurs > 0 && count < minOccurs)
                {
                    yield return new Diagnostic(Msg.D1025, item.Snapshots.First().SourceFile.RelativeFileName, TraceHelper.GetTextNode(item).TextSpan, Severity.Error, $"Item {GetText(schemaChildNode)} must occur at least {minOccurs} times");
                }

                if (maxOccurs > 0 && count > maxOccurs)
                {
                    yield return new Diagnostic(Msg.D1025, item.Snapshots.First().SourceFile.RelativeFileName, TraceHelper.GetTextNode(item).TextSpan, Severity.Error, $"Item {GetText(schemaChildNode)} must not occur more than {maxOccurs} times");
                }
            }

            // check unexpected item
            foreach (var child in children)
            {
                if (!schemaChildNodes.Any(c => IsMatch(child, c)))
                {
                    yield return new Diagnostic(Msg.D1025, item.Snapshots.First().SourceFile.RelativeFileName, TraceHelper.GetTextNode(item).TextSpan, Severity.Error, "Unexpected item: " + child.ItemIdOrPath);
                }
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
    }
}
