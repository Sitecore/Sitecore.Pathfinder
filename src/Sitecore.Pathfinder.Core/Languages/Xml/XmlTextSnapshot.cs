// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
    [Export]
    public class XmlTextSnapshot : TextSnapshot
    {
        [CanBeNull]
        private ITextNode _root;

        [FactoryConstructor]
        [ImportingConstructor]
        public XmlTextSnapshot([NotNull] ISnapshotService snapshotService) : base(snapshotService)
        {
        }

        public override ITextNode Root => _root ?? (_root = RootElement != null ? Parse(RootElement) : TextNode.Empty);

        [NotNull]
        public string SchemaFileName { get; private set; } = string.Empty;

        [NotNull]
        public string SchemaNamespace { get; private set; } = string.Empty;

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
        protected virtual ITextNode Parse([NotNull] XElement element)
        {
            var attributes = element.Attributes().Where(a => a.Name.LocalName != "xmlns").Select(a => new XmlTextNode(this, a)).ToArray();
            var childNodes = element.Elements().Select(Parse).ToArray();

            return new XmlTextNode(this, element, attributes, childNodes);
        }
    }
}
