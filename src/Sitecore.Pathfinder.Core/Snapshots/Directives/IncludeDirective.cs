// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Snapshots.Directives
{
    public class IncludeDirective : SnapshotDirectiveBase
    {
        public override bool CanParse(ITextNode textNode)
        {
            return textNode.Key == "File.Include";
        }

        public override IEnumerable<ITextNode> Parse(SnapshotParseContext snapshotParseContext, ITextNode textNode)
        {
            var mutableTextNode = textNode as IMutableTextNode;
            if (mutableTextNode == null)
            {
                throw new InvalidOperationException("Text node cannot be modified");
            }

            var textNodes = new List<ITextNode>();

            if (!textNode.Attributes.Any() && textNode.ChildNodes.Any())
            {
                foreach (var childNode in textNode.ChildNodes)
                {
                    var collection = Parse(snapshotParseContext, childNode);
                    if (collection != null)
                    {
                        textNodes.AddRange(collection);
                    }
                }

                return textNodes;
            }

            var fileName = textNode.GetAttributeValue("File");
            if (string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(textNode.Value))
            {
                fileName = textNode.Value;
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new InvalidOperationException("'File' attribute expected");
            }

            // todo: enable this check
            /*
            if (!string.Equals(Path.GetExtension(fileName), Path.GetExtension(textNode.Snapshot.SourceFile.AbsoluteFileName), StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Include file has a different format (Json/Xml/Yaml)");
            }
            */

            var placeholderTextNodes = new Dictionary<string, List<ITextNode>>(snapshotParseContext.PlaceholderTextNodes);

            var placeholdersTextNode = textNode.GetSnapshotLanguageSpecificChildNode("Placeholders");
            if (placeholdersTextNode != null)
            {
                foreach (var childNode in placeholdersTextNode.ChildNodes)
                {
                    var placeholderKey = childNode.GetAttributeValue("Key");
                    placeholderTextNodes[placeholderKey] = childNode.ChildNodes.ToList();
                }
            }

            mutableTextNode.ChildNodeCollection.Clear();

            var tokens = new Dictionary<string, string>(snapshotParseContext.Tokens).AddRange(textNode.Attributes.ToDictionary(a => a.Key, a => a.Value));
            var context = new SnapshotParseContext(snapshotParseContext.SnapshotService, snapshotParseContext.Project, tokens, placeholderTextNodes);

            textNodes.Add(snapshotParseContext.SnapshotService.LoadIncludeFile(context, textNode.Snapshot, fileName));

            return textNodes;
        }
    }
}
