// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Xml;
using System.Xml.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
    [Export]
    public class XmlTextSnapshot : TextSnapshot
    {
        [CanBeNull]
        private ITextNode _root;

        [ImportingConstructor]
        public XmlTextSnapshot([NotNull] ISnapshotService snapshotService) : base(snapshotService)
        {
        }

        public override ITextNode Root => _root ?? (_root = RootElement != null ? Parse(null, RootElement) : TextNode.Empty);

        [NotNull]
        public string SchemaFileName { get; private set; }

        [NotNull]
        public string SchemaNamespace { get; private set; }

        [CanBeNull]
        protected XElement RootElement { get; private set; }

        [NotNull]
        public virtual XmlTextSnapshot With([NotNull] ISourceFile sourceFile, [NotNull] string contents, [NotNull] string schemaNamespace, [NotNull] string schemaFileName)
        {
            base.With(sourceFile);

            SchemaNamespace = schemaNamespace;
            SchemaFileName = schemaFileName;

            try
            {
                var doc = XDocument.Parse(contents, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
                RootElement = doc.Root;
            }
            catch (XmlException ex)
            {
                ParseError = ex.Message;
                ParseErrorTextSpan = new TextSpan(ex.LineNumber, ex.LinePosition, 0);
                RootElement = null;
            }
            catch (Exception ex)
            {
                ParseError = ex.Message;
                RootElement = null;
            }

            return this;
        }

        [NotNull]
        protected virtual ITextNode Parse([CanBeNull] ITextNode parent, [NotNull] XElement element)
        {
            var childNodes = (ICollection<ITextNode>)parent?.ChildNodes;

            var treeNode = new XmlTextNode(this, element);
            childNodes?.Add(treeNode);

            var attributes = (ICollection<ITextNode>)treeNode.Attributes;

            foreach (var attribute in element.Attributes())
            {
                if (attribute.Name.LocalName == "xmlns")
                {
                    continue;
                }

                var attributeTreeNode = new XmlTextNode(this, attribute);
                attributes.Add(attributeTreeNode);
            }

            foreach (var child in element.Elements())
            {
                Parse(treeNode, child);
            }

            return treeNode;
        }
    }
}
