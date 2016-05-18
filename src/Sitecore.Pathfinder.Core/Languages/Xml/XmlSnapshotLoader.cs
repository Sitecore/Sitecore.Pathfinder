// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
    public class XmlSnapshotLoader : SnapshotLoaderBase
    {
        [ImportingConstructor]
        public XmlSnapshotLoader([NotNull] ICompositionService compositionService, [NotNull] IFileSystemService fileSystem)
        {
            CompositionService = compositionService;
            FileSystem = fileSystem;
            Priority = 1000;
        }

        [NotNull]
        public string SchemaFileName { get; protected set; } = string.Empty;

        [NotNull]
        public string SchemaNamespace { get; protected set; } = string.Empty;

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override bool CanLoad(ISourceFile sourceFile)
        {
            return string.Equals(Path.GetExtension(sourceFile.AbsoluteFileName), ".xml", StringComparison.OrdinalIgnoreCase);
        }

        public override ISnapshot Load(SnapshotParseContext snapshotParseContext, ISourceFile sourceFile)
        {
            var text = sourceFile.ReadAsText(snapshotParseContext.Tokens);

            var contents = Transform(snapshotParseContext, sourceFile, text);

            return CompositionService.Resolve<XmlTextSnapshot>().With(snapshotParseContext, sourceFile, contents, SchemaNamespace, SchemaFileName);
        }

        [NotNull]
        protected virtual string Transform([NotNull] SnapshotParseContext snapshotParseContext, [NotNull] ISourceFile sourceFile, [NotNull] string text)
        {
            var extensions = PathHelper.GetExtension(sourceFile.AbsoluteFileName);

            // cut off .xml
            extensions = extensions.Left(extensions.Length - 4);
            if (string.IsNullOrEmpty(extensions))
            {
                return text;
            }

            var transformDirectory = Path.Combine(snapshotParseContext.Project.Options.ProjectDirectory, "sitecore.project\\transforms");
            if (!FileSystem.DirectoryExists(transformDirectory))
            {
                return text;
            }

            // use extensions as base filename for the xslt, e.g. "myproject/xml/home.pagetype.xml" -> "myproject/sitecore.project/transforms/pagetype.xslt"
            var fileName = extensions.Mid(1) + ".xslt";
            var xsltFileName = Path.Combine(transformDirectory, fileName);
            if (!FileSystem.FileExists(xsltFileName))
            {
                return text;
            }

            return Transform(xsltFileName, text);
        }

        [NotNull]
        protected virtual string Transform([NotNull] string xsltFileName, [NotNull] string text)
        {
            var xmlReader = new XmlTextReader(new StringReader(text));
            var xpathDocument = new XPathDocument(xmlReader);

            using (var streamReader = new StreamReader(xsltFileName))
            {
                var xsltReader = new XmlTextReader(streamReader);

                // todo: cache transform
                var transform = new XslCompiledTransform();
                transform.Load(xsltReader);

                var writer = new StringBuilder();
                TextWriter output = new StringWriter(writer);

                transform.Transform(xpathDocument, null, output);

                return writer.ToString();
            }
        }
    }
}
