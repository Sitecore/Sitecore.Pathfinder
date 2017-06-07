// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.Text;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Parsing.References;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Serialization
{
    [Export(typeof(ICompiler)), Shared]
    public class SerializationFileCompiler : CompilerBase
    {
        [ImportingConstructor]
        public SerializationFileCompiler([NotNull] ITraceService trace, [NotNull] IReferenceParserService referenceParser) : base(1000)
        {
            Trace = trace;
            ReferenceParser = referenceParser;
        }

        [NotNull]
        protected IReferenceParserService ReferenceParser { get; }

        [NotNull]
        protected ITraceService Trace { get; }

        public override bool CanCompile(ICompileContext context, IProjectItem projectItem)
        {
            return projectItem is SerializationFile;
        }

        public override void Compile(ICompileContext context, IProjectItem projectItem)
        {
            var serializationFile = projectItem as SerializationFile;
            Assert.Cast(serializationFile, nameof(serializationFile));

            var textDocument = (ITextSnapshot)serializationFile.Snapshot;
            var rootTextNode = textDocument.Root;
            if (rootTextNode == TextNode.Empty)
            {
                Trace.TraceError(Msg.C1050, Texts.Document_is_not_valid, textDocument.SourceFile.AbsoluteFileName, TextSpan.Empty);
                return;
            }

            var lines = textDocument.SourceFile.ReadAsLines();
            var itemBuilder = new ItemBuilder(context.Factory);

            ParseItem(context, textDocument, itemBuilder, lines);

            var item = itemBuilder.Build(serializationFile.Project, rootTextNode);
            item.IsEmittable = false;

            item.References.AddRange(ReferenceParser.ParseReferences(item, item.TemplateIdOrPathProperty));
            foreach (var field in item.Fields)
            {
                item.References.AddRange(ReferenceParser.ParseReferences(field));
            }

            var addedItem = context.Project.AddOrMerge(item);
            serializationFile.SerializationItemUri = addedItem.Uri;
        }

        protected virtual TextSpan GetTextSpan(int lineNumber, int linePosition, int length)
        {
            return new TextSpan(lineNumber + 1, linePosition, length);
        }

        protected virtual int ParseField([NotNull] ICompileContext context, [NotNull] ITextSnapshot textSnapshot, [NotNull] ItemBuilder itemBuilder, [NotNull] LanguageVersionBuilder languageVersionBuilder, [NotNull, ItemNotNull] string[] lines, int lineNumber)
        {
            var textNode = context.Factory.TextNode(textSnapshot, string.Empty, string.Empty, GetTextSpan(lineNumber, 0, 0));

            var fieldBuilder = new FieldBuilder(context.Factory).With(itemBuilder, textNode);
            itemBuilder.Fields.Add(fieldBuilder);

            fieldBuilder.Language = languageVersionBuilder.Language;
            fieldBuilder.LanguageTextNode = languageVersionBuilder.LanguageTextNode;

            fieldBuilder.Version = languageVersionBuilder.Version;
            fieldBuilder.VersionTextNode = languageVersionBuilder.VersionTextNode;

            var lineLength = 0;

            int n;
            for (n = lineNumber; n < lines.Length; n++)
            {
                var line = lines[n];
                lineLength += line.Length;

                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var i = line.IndexOf(':');
                if (i < 0)
                {
                    break;
                }

                var name = line.Left(i).Trim();
                var value = line.Mid(i + 1).Trim();

                switch (name)
                {
                    case "field":
                        fieldBuilder.FieldId = value;
                        fieldBuilder.FieldIdTextNode = context.Factory.TextNode(textSnapshot, "field", value, GetTextSpan(lineNumber, 0, lineLength));
                        break;
                    case "name":
                        fieldBuilder.FieldName = value;
                        fieldBuilder.FieldNameTextNode = context.Factory.TextNode(textSnapshot, "name", value, GetTextSpan(lineNumber, 0, lineLength));
                        break;
                    case "key": break;
                }

                if (name == "content-length")
                {
                    var contentLength = int.Parse(value);
                    n = ParseFieldValue(context, textSnapshot, lines, fieldBuilder, n + 2, contentLength, ref lineLength);
                    break;
                }
            }

            return n;
        }

        protected virtual int ParseFieldValue([NotNull] ICompileContext context, [NotNull] ITextSnapshot textSnapshot, [NotNull, ItemNotNull] string[] lines, [NotNull] FieldBuilder fieldBuilder, int startIndex, int contentLength, ref int lineLength)
        {
            string value;
            var sb = new StringBuilder();

            for (var n = startIndex; n < lines.Length; n++)
            {
                var line = lines[n];
                lineLength += line.Length;

                if (sb.Length < contentLength)
                {
                    sb.Append(line);
                    sb.Append("\r\n");
                    continue;
                }

                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                value = sb.ToString().Trim().TrimEnd('\n', '\r');
                fieldBuilder.Value = value;
                fieldBuilder.ValueTextNode = context.Factory.TextNode(textSnapshot, string.Empty, value, GetTextSpan(startIndex, 0, contentLength));
                return n - 1;
            }

            value = sb.ToString().Trim().TrimEnd('\n', '\r');
            fieldBuilder.Value = value;
            fieldBuilder.ValueTextNode = context.Factory.TextNode(textSnapshot, string.Empty, value, GetTextSpan(startIndex, 0, contentLength));
            return lines.Length;
        }

        protected virtual int ParseItem([NotNull] ICompileContext context, [NotNull] ITextSnapshot textSnapshot, [NotNull] ItemBuilder itemBuilder, [NotNull, ItemNotNull] string[] lines)
        {
            var languageVersionBuilder = new LanguageVersionBuilder();

            for (var n = 0; n < lines.Length; n++)
            {
                var line = lines[n];
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                if (line == "----field----")
                {
                    n = ParseField(context, textSnapshot, itemBuilder, languageVersionBuilder, lines, n + 1);
                    continue;
                }

                if (line == "----version----")
                {
                    n = ParseVersion(context, context.Project.GetDatabase(itemBuilder.DatabaseName), textSnapshot, languageVersionBuilder, lines, n + 1);
                    continue;
                }

                if (line == "----item----")
                {
                    continue;
                }

                var i = line.IndexOf(':');
                if (i < 0)
                {
                    return n;
                }

                var name = line.Left(i).Trim();
                var value = line.Mid(i + 1).Trim();

                switch (name)
                {
                    case "id":
                        itemBuilder.Guid = value;
                        break;
                    case "database":
                        itemBuilder.DatabaseName = value;
                        break;
                    case "path":
                        itemBuilder.ItemIdOrPath = value;
                        break;
                    case "parent": break;
                    case "name":
                        itemBuilder.ItemName = value;
                        itemBuilder.ItemNameTextNode = context.Factory.TextNode(textSnapshot, "name", value, GetTextSpan(n, 0, line.Length));
                        break;
                    case "master": break;
                    case "template":
                        itemBuilder.TemplateIdOrPath = value;
                        itemBuilder.TemplateIdOrPathTextNode = context.Factory.TextNode(textSnapshot, "template", value, GetTextSpan(n, 0, line.Length));
                        break;
                    case "templatekey": break;
                    case "version": break;
                }
            }

            return lines.Length;
        }

        protected virtual int ParseVersion([NotNull] ICompileContext context, [NotNull] Database database, [NotNull] ITextSnapshot textSnapshot, [NotNull] LanguageVersionBuilder languageVersionBuilder, [NotNull, ItemNotNull] string[] lines, int lineNumber)
        {
            for (var n = lineNumber; n < lines.Length; n++)
            {
                var line = lines[n];
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var i = line.IndexOf(':');
                if (i < 0)
                {
                    return n - 1;
                }

                var name = line.Left(i).Trim();
                var value = line.Mid(i + 1).Trim();

                switch (name)
                {
                    case "language":
                        languageVersionBuilder.Language = database.GetLanguage(value);
                        languageVersionBuilder.LanguageTextNode = context.Factory.TextNode(textSnapshot, "language", value, GetTextSpan(n, 0, line.Length));
                        break;
                    case "version":
                        if (Version.TryParse(value, out Version version))
                        {
                            languageVersionBuilder.Version = version;
                            languageVersionBuilder.VersionTextNode = context.Factory.TextNode(textSnapshot, "version", value, GetTextSpan(n, 0, line.Length));
                        }
                        break;
                    case "revision": break;
                }
            }

            return lines.Length;
        }
    }
}
