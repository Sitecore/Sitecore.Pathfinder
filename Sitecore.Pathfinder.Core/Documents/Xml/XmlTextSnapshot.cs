// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing;

namespace Sitecore.Pathfinder.Documents.Xml
{
    public class XmlTextSnapshot : TextSnapshot
    {
        protected static readonly Dictionary<string, XmlSchemaSet> Schemas = new Dictionary<string, XmlSchemaSet>();

        private ITextNode _root;

        private XElement _rootElement;

        public XmlTextSnapshot([NotNull] ISourceFile sourceFile, [NotNull] string contents, [NotNull] string schemaNamespace, [NotNull] string schemaFileName) : base(sourceFile, contents)
        {
            SchemaNamespace = schemaNamespace;
            SchemaFileName = schemaFileName;
            IsEditable = true;
        }

        public override ITextNode Root => _root ?? (_root = (RootElement != null ? Parse(null, RootElement) : TextNode.Empty));

        [NotNull]
        public string SchemaFileName { get; }

        [NotNull]
        public string SchemaNamespace { get; }

        [CanBeNull]
        protected XElement RootElement
        {
            get
            {
                if (_rootElement != null)
                {
                    return _rootElement;
                }

                XDocument doc;
                try
                {
                    doc = XDocument.Parse(Contents, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
                }
                catch
                {
                    return null;
                }

                _rootElement = doc.Root;
                return _rootElement;
            }
        }

        public override void BeginEdit()
        {
            IsEditing = true;
        }

        public override void EndEdit()
        {
            if (!IsEditing)
            {
                throw new InvalidOperationException("Document is not in edit mode");
            }

            if (_root == null)
            {
                return;
            }

            IsEditing = false;
            _rootElement.Save(SourceFile.FileName, SaveOptions.DisableFormatting);
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

            ValidationEventHandler validate = delegate(object sender, ValidationEventArgs args)
            {
                switch (args.Severity)
                {
                    case XmlSeverityType.Error:
                        context.Trace.TraceError(string.Empty, context.Snapshot.SourceFile.FileName, new TextPosition(args.Exception.LineNumber, args.Exception.LinePosition, 0), args.Message);
                        break;
                    case XmlSeverityType.Warning:
                        context.Trace.TraceWarning(string.Empty, context.Snapshot.SourceFile.FileName, new TextPosition(args.Exception.LineNumber, args.Exception.LinePosition, 0), args.Message);
                        break;
                }
            };

            try
            {
                doc.Validate(schema, validate);
            }
            catch (Exception ex)
            {
                context.Trace.TraceError(Texts.The_file_does_not_contain_valid_XML, context.Snapshot.SourceFile.FileName, TextPosition.Empty, ex.Message);
            }
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
            var treeNode = new XmlTextNode(this, element, parent);
            (parent?.ChildNodes as ICollection<ITextNode>)?.Add(treeNode);

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

            if (!element.HasElements)
            {
                var node = element.Nodes().FirstOrDefault();
                if (node != null)
                {
                    var attributeTreeNode = new XmlTextNode(this, node, "[Value]", element.Value);
                    attributes.Add(attributeTreeNode);
                }
            }

            foreach (var child in element.Elements())
            {
                Parse(treeNode, child);
            }

            return treeNode;
        }
    }
}
