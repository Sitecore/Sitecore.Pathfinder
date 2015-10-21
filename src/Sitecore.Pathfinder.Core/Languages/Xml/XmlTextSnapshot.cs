// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class XmlTextSnapshot : TextSnapshot
    {
        [NotNull]
        protected static readonly Dictionary<string, XmlSchemaSet> Schemas = new Dictionary<string, XmlSchemaSet>();

        [CanBeNull]
        private ITextNode _root;

        [ImportingConstructor]
        public XmlTextSnapshot([NotNull] ISnapshotService snapshotService)
        {
            SnapshotService = snapshotService;
        }

        public override ITextNode Root => _root ?? (_root = (RootElement != null ? Parse(null, RootElement) : TextNode.Empty));

        [NotNull]
        public string SchemaFileName { get; private set; }

        [NotNull]
        public string SchemaNamespace { get; private set; }

        [NotNull]
        protected IProject Project { get; private set; }

        [CanBeNull]
        protected XElement RootElement { get; private set; }

        [NotNull]
        protected ISnapshotService SnapshotService { get; }

        [NotNull]
        protected IDictionary<string, string> Tokens { get; private set; }

        public override void SaveChanges()
        {
            if (RootElement == null)
            {
                return;
            }

            using (var writer = new StreamWriter(SourceFile.AbsoluteFileName))
            {
                RootElement.Save(writer, SaveOptions.DisableFormatting);
            }
        }

        public override void ValidateSchema(IParseContext context)
        {
            if (string.IsNullOrEmpty(SchemaFileName) || string.IsNullOrEmpty(SchemaNamespace))
            {
                return;
            }

            var doc = RootElement?.Document;
            if (doc == null)
            {
                return;
            }

            XmlSchemaSet schema;
            if (!Schemas.TryGetValue(SchemaNamespace, out schema))
            {
                schema = GetSchema(context, SchemaFileName, SchemaNamespace);
                Schemas[SchemaNamespace] = schema;
            }

            if (schema == null)
            {
                return;
            }

            ValidationEventHandler validateHandler = delegate(object sender, ValidationEventArgs args)
            {
                var length = 0;
                var element = sender as XElement;
                if (element != null)
                {
                    length = element.Name.LocalName.Length;
                }

                switch (args.Severity)
                {
                    case XmlSeverityType.Error:
                        context.Trace.TraceError(string.Empty, context.Snapshot.SourceFile.AbsoluteFileName, new TextSpan(args.Exception.LineNumber, args.Exception.LinePosition, length), args.Message);
                        break;
                    case XmlSeverityType.Warning:
                        context.Trace.TraceWarning(string.Empty, context.Snapshot.SourceFile.AbsoluteFileName, new TextSpan(args.Exception.LineNumber, args.Exception.LinePosition, length), args.Message);
                        break;
                }
            };

            try
            {
                doc.Validate(schema, validateHandler);
            }
            catch (Exception ex)
            {
                context.Trace.TraceError(Texts.The_file_does_not_contain_valid_XML, context.Snapshot.SourceFile.AbsoluteFileName, TextSpan.Empty, ex.Message);
            }
        }

        [NotNull]
        public virtual XmlTextSnapshot With([NotNull] ISourceFile sourceFile, [NotNull] string contents, [NotNull] IDictionary<string, string> tokens, [NotNull] string schemaNamespace, [NotNull] string schemaFileName)
        {
            base.With(sourceFile);

            SchemaNamespace = schemaNamespace;
            SchemaFileName = schemaFileName;
            Tokens = tokens;

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

        [CanBeNull]
        protected virtual XmlSchemaSet GetSchema([NotNull] IParseContext context, [NotNull] string schemaFileName, [NotNull] string schemaNamespace)
        {
            var fileName = Path.Combine(context.Configuration.Get(Constants.Configuration.ToolsDirectory), "schemas\\" + schemaFileName);
            if (!context.Project.FileSystem.FileExists(fileName))
            {
                return null;
            }

            var schemas = new XmlSchemaSet();
            schemas.Add(schemaNamespace, fileName);

            return schemas;
        }

        [NotNull]
        protected virtual ITextNode Parse([CanBeNull] ITextNode parent, [NotNull] XElement element)
        {
            var childNodes = (ICollection<ITextNode>)parent?.ChildNodes;

            if (element.Name.LocalName == "Include")
            {
                return childNodes == null ? TextNode.Empty : ParseIncludeFile(element, childNodes);
            }

            var treeNode = new XmlTextNode(this, element, parent);
            childNodes?.Add(treeNode);

            var attributes = (ICollection<ITextNode>)treeNode.Attributes;

            foreach (var attribute in element.Attributes())
            {
                if (attribute.Name.LocalName == "xmlns")
                {
                    continue;
                }

                var attributeTreeNode = new XmlTextNode(this, attribute, treeNode);
                attributes.Add(attributeTreeNode);
            }

            foreach (var child in element.Elements())
            {
                Parse(treeNode, child);
            }

            return treeNode;
        }

        [NotNull]
        protected virtual ITextNode ParseIncludeFile([NotNull] XElement element, [NotNull] [ItemNotNull] ICollection<ITextNode> childNodes)
        {
            var fileName = element.GetAttributeValue("File");
            if (string.IsNullOrEmpty(fileName))
            {
                throw new InvalidOperationException("'File' attribute expected");
            }

            var tokens = new Dictionary<string, string>(Tokens).AddRange(element.Attributes().ToDictionary(a => a.Name.LocalName, a => a.Value));

            var textNode = SnapshotService.LoadIncludeFile(this, fileName, tokens);
            if (textNode != TextNode.Empty)
            {
                childNodes.Add(textNode);
            }

            return textNode;
        }
    }
}
